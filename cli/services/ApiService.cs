﻿using cli.models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace cli.services
{
  internal class ApiService
  {
    private readonly HttpClient httpClient = new()
    {
      Timeout = TimeSpan.FromSeconds(60 * 10)
    };

    public ApiService(LocalPreferencesServices preferencesServices)
    {
      var currentUser = preferencesServices.GetUser();
      if (currentUser != null)
      {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);
      }
    }

    private static string BuildUrl(string path) => $"{Assembly.ApiUrl}/{path}";

    private async Task<R> Post<T, R>(string path, T body) where T : class where R : class
    {
      string jsonStr = JsonSerializer.Serialize(body);
      StringContent content = new(jsonStr, Encoding.UTF8, "application/json");

      var response = await httpClient.PostAsync(BuildUrl(path), content);
      var stringResponse = await response.Content.ReadAsStringAsync();

      if (response.IsSuccessStatusCode)
      {
        return JsonSerializer.Deserialize<R>(stringResponse)!;
      }
      else
      {
        throw new ApiException($"{stringResponse}: Status code {response.StatusCode}.");
      }
    }

    private async Task<R> Put<T, R>(string path, T body) where T : class where R : class
    {
      string jsonStr = JsonSerializer.Serialize(body);
      StringContent content = new(jsonStr, Encoding.UTF8, "application/json");
      var response = await httpClient.PutAsync(BuildUrl(path), content);
      var stringResponse = await response.Content.ReadAsStringAsync();
      if (response.IsSuccessStatusCode)
      {
        return JsonSerializer.Deserialize<R>(stringResponse)!;
      }
      else
      {
        throw new ApiException($"{stringResponse}: Status code {response.StatusCode}.");
      }
    }

    private async Task<T> Get<T>(string path) where T : class
    {
      var response = await httpClient.GetAsync(BuildUrl(path));
      var stringResponse = await response.Content.ReadAsStringAsync();

      if (response.IsSuccessStatusCode)
      {
        return JsonSerializer.Deserialize<T>(stringResponse)!;
      }
      else
      {
        throw new ApiException($"{stringResponse}: Status code {response.StatusCode}.");
      }
    }

    public Task<Project> CreateProject(string projectName, string githubRepo, string Token, bool publicProject)
    {
      return Post<Dictionary<string, object>, Project>("projects", new Dictionary<string, object>{
                {"ProjectName", projectName},
                {"Repo", githubRepo},
                {"Token", Token},
                {"PublicProject", publicProject}
            });
    }

    public Task<Project> UpdateProject(int projectId, string projectName, string githubRepo, string token, bool publicProject)
    {
      return Put<Dictionary<string, object>, Project>("projects", new Dictionary<string, object>{
                {"Id", projectId},
                {"ProjectName", projectName},
                {"Repo", githubRepo},
                {"Token", token},
                {"PublicProject", publicProject}
            });
    }

    public ReleaseTester UpdateReleaseTester(int releaseTesterId, int state, string comment)
    {
      return Put<Dictionary<string, object>, ReleaseTester>("release-testers", new Dictionary<string, object>{
                {"Id", releaseTesterId},
                {"State", state},
                {"Comment", comment}
            }).Result;
    }

    public Task<UserProject> AddTester(string email, int projectId)
    {
      return Post<Dictionary<string, string>, UserProject>("user-projects", new Dictionary<string, string>{
                {"Email", email},
                {"ProjectId", projectId.ToString()},
                {"Role", "1"}
            });
    }

    public Task<User> LoginUser(string accessCode)
    {
      return Post<Dictionary<string, string>, User>("users", new Dictionary<string, string>{
                {"AccessCode", accessCode}
            });
    }

    public Task<Project> GetProjectById(int projectId)
    {
      return Get<Project>($"projects/{projectId}");
    }

    public Task<List<Project>> GetProjectsByUserId(int userId)
    {
      return Get<List<Project>>($"projects/user/{userId}");
    }

    public Task<List<PendingRelease>> GetPendingReleasesByUserId(int userId)
    {
      return Get<List<PendingRelease>>($"releases/user/{userId}");
    }

    public Task<Release> CreateRelease(string releaseName, int projectId, string downloadLink)
    {
      return Post<Dictionary<string, object>, Release>("releases", new Dictionary<string, object>{
                {"Name", releaseName},
                {"ProjectId", projectId},
                {"DownloadLink", downloadLink}
            });
    }

    public Task<List<Release>> FetchReleases(int projectId)
    {
      return Get<List<Release>>($"releases/project/{projectId}");
    }

    public Task<List<string>> FetchRepos()
    {
      return Get<List<string>>("users/me/repos");
    }
  }

  public sealed class ApiException(string message) : Exception(message)
  {
  }
}
