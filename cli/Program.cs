using cli;
using cli.services;
using ReleaseMonkey.Server.Services;

internal class Program
{
  private static async Task Main(string[] args)
  {
    try
    {
      await ParseArgs(args);
    }
    catch (ApiException e)
    {
      Console.WriteLine(e.Message);
    }
  }

  private static async Task ParseArgs(string[] args)
  {
    LocalPreferencesServices localPreferencesServices = new();
    ApiService apiService = new(localPreferencesServices);
    GithubService githubService = new("Iv1.2a4a99768f6b514e", 3001);
    Commands commands = new(localPreferencesServices, apiService, githubService);

    if (args.Length > 0)
    {
      switch (args[0])
      {
        case "login":
          await commands.LoginWithGithub();
          break;
        case "logout":
          await commands.Logout();
          break;
        case "user":
          await commands.PrintCurrentUser();
          break;
        case "create-project":
          if (args.Length > 4)
          {
            if (args[4].ToLower().Equals("true"))
            {
              await commands.CreateProject(args[1], args[2], args[3], true);
            }
            else if (args[4].ToLower().Equals("false"))
            {
              await commands.CreateProject(args[1], args[2], args[3], false);
            }
            else
            {
              Console.WriteLine("Please enter true/false for third argument.");
            }
          }
          else if (args.Length > 3)
          {
            await commands.CreateProject(args[1], args[2], args[3], false);
          }
          else
          {
            Console.WriteLine("Please provide the project name followed by the Github repo (owner/repo_name) followed by a git personal access token and optionally whether the project should be public or not to create a project.");
          }
          break;
        case "public-project":
          if (args.Length > 1)
          {
            if (args[1].ToLower().Equals("true"))
            {
              await commands.PublicProject(true);
            }
            else if (args[1].ToLower().Equals("false"))
            {
              await commands.PublicProject(false);
            }
            else
            {
              Console.WriteLine("Please enter true/false for third argument.");
            }
          }
          break;
        case "set-project":
          if (args.Length > 1)
          {
            await commands.SetProject(args[1]);
          }
          else
          {
            Console.WriteLine("Please provide the project id.");
          }
          break;
        case "project":
          await commands.PrintProject();
          break;
        case "list-projects":
          await commands.ListProjects();
          break;
        case "list-pending":
          await commands.ListPendingReleases();
          break;
        case "add-testers":
          if (args.Length > 1)
          {
            var emails = args.Skip(1).ToList();
            await commands.AddTesters(emails);
          }
          else
          {
            Console.WriteLine("Please provide at least one email address to add as tester.");
          }
          break;
        case "create-release":
          if (args.Length > 2)
          {
            await commands.CreateRelease(args[1], args[2]);
          }
          else
          {
            Console.WriteLine("Please provide a name and a download link for the release.");
          }
          break;
        case "list-releases":
          await commands.ListReleases();
          break;
        case "approve-release":
          try
          {
            int i = Int32.Parse(args[1]);
            if (args.Length > 2)
            {
              await commands.UpdateReleaseTester(i, 0, args[2]);
              Console.WriteLine($"Release has been approved");
            }
            else if (args.Length > 1)
            {
              await commands.UpdateReleaseTester(i, 0, "");
              Console.WriteLine($"Release has been approved");
            }
            else
            {
              Console.WriteLine("Please provide the id of the release to approve.");
            }
          }
          catch
          {
            Console.WriteLine("Please provide the id of the release to approve.");
          }
          break;
        case "reject-release":
          try
          {
            int i = Int32.Parse(args[1]);
            if (args.Length > 2)
            {
              await commands.UpdateReleaseTester(i, 1, args[2]);
              Console.WriteLine($"Release has been rejected");
            }
            else if (args.Length > 1)
            {
              await commands.UpdateReleaseTester(i, 1, "");
              Console.WriteLine($"Release has been rejected");
            }
            else
            {
              Console.WriteLine("Please provide a valid number for the id of the release to reject.");
            }
          }
          catch
          {
            Console.WriteLine("Please provide the id of the release to reject and optionally a comment.");
          }
          break;
        case "release-key":
          await commands.PrintReleaseKey();
          break;
        case "load-release-key":
          if (args.Length > 1)
          {
            await commands.LoadReleaseKey(args[1]);
          }
          else
          {
            Console.WriteLine("Please provide a release key to load.");
          }
          break;
        case "repos":
          await commands.ListRepos();
          break;
        case "version":
          commands.PrintVersion();
          break;
        case "help":
          commands.PrintHelp();
          commands.PrintCommands();
          break;
        default:
          Console.WriteLine("Invalid command given.");
          commands.PrintHelp();
          break;
      }
    }
    else
    {
      commands.PrintHelp();
    }
  }
}
