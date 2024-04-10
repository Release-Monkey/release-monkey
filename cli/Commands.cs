using cli.services;
using ReleaseMonkey.Server.Services;
using cli.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace cli
{
  internal class Commands(LocalPreferencesServices preferencesServices, ApiService apiService, GithubService githubService)
  {
    public async Task LoginWithGithub()
    {
      var accessCode = await githubService.GetAccessCode();
      var user = await apiService.LoginUser(accessCode);
      preferencesServices.SetUser(user);

      Console.WriteLine($"Signed in as {user.Name}, {user.Email}.");
    }

    public Task Logout()
    {
      preferencesServices.ClearUser();
      preferencesServices.ClearProject();
      Console.WriteLine("Signed out. You will have to sign in again to publish releases.");
      return Task.FromResult<object?>(null);
    }

    public Task PrintCurrentUser()
    {
      var user = preferencesServices.GetUser();
      if (user == null)
      {
        Console.WriteLine("Not signed in.");
      }
      else
      {
        Console.WriteLine($"Signed in as {user.Name}, {user.Email}.");
      }

      return Task.FromResult<object?>(null);
    }

    public async Task CreateProject(string projectName, string githubRepo, string token, bool publicProject)
    {
      try
      {
        var project = await apiService.CreateProject(projectName, githubRepo, token, publicProject);
        preferencesServices.SetProject(project);
        Console.WriteLine($"Project has been created. Project id is {project.Id}.");
      }
      catch (ApiException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    public async Task PublicProject(bool publicProject)
    {
      try
      {
        var oldProject = preferencesServices.GetProject();
        if (oldProject != null)
        {
          var project = await apiService.UpdateProject(oldProject.Id, oldProject.Name, oldProject.Repo, oldProject.Token, publicProject);
          preferencesServices.SetProject(project);
          if (publicProject)
          {
            Console.WriteLine($"Project has been made public.");
          }
          else
          {
            Console.WriteLine($"Project has been made private.");
          }
        }
        else
        {
          Console.WriteLine($"No project has been set");
        }

      }
      catch (ApiException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    public async Task SetProject(string projectId)
    {
      try
      {
        var parsedProjectId = int.Parse(projectId);
        var project = await apiService.GetProjectById(parsedProjectId);
        preferencesServices.SetProject(project);
        Console.WriteLine($"Project has been set to {project.Name}, publishing {project.Repo}.");
      }
      catch (FormatException)
      {
        Console.WriteLine("Please enter a valid project id.");
      }

    }

    public Task PrintProject()
    {
      var project = preferencesServices.GetProject();
      if (project == null)
      {
        Console.WriteLine("No project set. Either create a new project or set project with 'set-project PROJECT_ID'.");
      }
      else
      {
        Console.WriteLine($"Current project is {project!.Name}, publishing {project.Repo}.");
      }

      return Task.FromResult<object?>(null);
    }

    public Task ListProjects() { throw new NotImplementedException(); }

    public async Task AddTesters(List<string> testerEmails)
    {
      var project = preferencesServices.GetProject();
      if (project == null)
      {
        Console.WriteLine("No project set. Either create a new project or set project with 'set-project PROJECT_ID'.");
      }
      else
      {
        Console.WriteLine($"Current project is {project!.Name}, publishing {project.Repo}.");
        foreach (string email in testerEmails)
        {
          try
          {
            await apiService.AddTester(email, project.Id);
            Console.WriteLine($"Tester {email} has been added to {project.Name}.");
          }
          catch (ApiException e)
          {
            Console.WriteLine(e.Message);

          }
        }
      }
    }

    public async Task CreateRelease(string releaseName, string downloadLink)
    {
      var currentProject = preferencesServices.GetProject();
      if (currentProject == null)
      {
        Console.WriteLine("No project set. Either create a new project or set project with 'set-project PROJECT_ID'.");
      }
      else
      {
        var release = await apiService.CreateRelease(releaseName, currentProject.Id, downloadLink);
        Console.WriteLine($"New release, {release.ReleaseName}, has been created for {currentProject.Name}. Your testers will be notified via email to begin testing.");
      }
    }

    public async Task ListReleases()
    {
      var currentProject = preferencesServices.GetProject();
      if (currentProject == null)
      {
        Console.WriteLine("No project set. Either create a new project or set project with 'set-project PROJECT_ID'.");
      }
      else
      {
        var releases = await apiService.FetchReleases(currentProject.Id);
        Console.WriteLine($"Releases for {currentProject.Name}:");
        releases.ForEach(release => Console.WriteLine($"{release.ReleaseName} ({release.Id})"));
      }
    }

    public Task PrintReleaseKey()
    {
      var user = preferencesServices.GetUser();
      if (user == null)
      {
        Console.WriteLine("Not signed in. Please sign in to create release keys.");
      }
      else
      {
        var project = preferencesServices.GetProject();
        if (project == null)
        {
          Console.WriteLine("No project set. Either create a new project or set project with 'set-project PROJECT_ID'.");
        }
        else
        {
          ReleaseKey releaseKey = new(user, project);
          var jsonString = JsonSerializer.Serialize(releaseKey);
          var jsonStringBytes = Encoding.UTF8.GetBytes(jsonString);
          Console.WriteLine(Convert.ToBase64String(jsonStringBytes));
        }
      }

      return Task.FromResult<object?>(null);
    }

    public Task LoadReleaseKey(string releaseKeyString)
    {
      var releaseKeyStringBytes = Convert.FromBase64String(releaseKeyString);
      var releaseKeyJson = Encoding.UTF8.GetString(releaseKeyStringBytes);
      var releaseKey = JsonSerializer.Deserialize<ReleaseKey>(releaseKeyJson);
      if (releaseKey == null)
      {
        Console.WriteLine("Invalid release key provided. Could not decode.");
      }
      else
      {
        preferencesServices.SetProject(releaseKey.Project);
        preferencesServices.SetUser(releaseKey.User);
      }

      return Task.FromResult<object?>(null);
    }

    public Task UpdateReleaseTester(int releaseId, int state, string comment)
    {
      if (state == 0)
      {
        var releaseTester = apiService.UpdateReleaseTester(releaseId, state, comment);
        return Task.FromResult<object>(releaseTester);
      } else if (state == 1)
      {
        var releaseTester = apiService.UpdateReleaseTester(releaseId, state, comment);
        return Task.FromResult<object>(releaseTester);
      } else
      {
        Console.WriteLine($"Inserted state is incorrect.");
        return Task.FromResult<object?>(null);
      }
      
    }

    public async Task ListRepos()
    {
      var repos = await apiService.FetchRepos();
      repos.ForEach(Console.WriteLine);
    }

    public void PrintVersion()
    {
      Console.WriteLine($"Release Monkey {Assembly.Version}\n{Assembly.GetBuild()} build");
    }

    public void PrintHelp()
    {
      Console.WriteLine("Welcome to Release Monkey. Use CLI to do everything. More information available in README at https://github.com/Release-Monkey/release-monkey.");
    }
  }
}
