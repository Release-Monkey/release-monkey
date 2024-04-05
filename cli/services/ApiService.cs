﻿using cli.models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace cli.services
{
    internal class ApiService
    {
        private readonly HttpClient httpClient = new();

        public ApiService(LocalPreferencesServices preferencesServices)
        {
            var currentUser = preferencesServices.GetUser();
            if (currentUser != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);
            }
        }

        private static string BuildUrl(string path) => $"http://localhost:3000/{path}";

        private async Task<R> Post<T, R>(string path, T body) where T : class where R : class
        {
            string jsonStr = JsonSerializer.Serialize(body);
            StringContent content = new(jsonStr, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(BuildUrl(path), content);
            var stringResponse = await response.Content.ReadAsStringAsync();

            if (response.StatusCode >= HttpStatusCode.OK && response.StatusCode <= HttpStatusCode.PartialContent)
            {
                return JsonSerializer.Deserialize<R>(stringResponse)!;
            }
            else
            {
                // TODO: Do proper error reporting.
                throw new ApiException($"{stringResponse}: Status code {response.StatusCode}");
            }
        }

        private async Task<T> Get<T>(string path) where T : class
        {
            var response = await httpClient.GetAsync(BuildUrl(path));
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(stringResponse)!;
        }

        public Task<Project> CreateProject(string projectName, string githubRepo)
        {
            return Post<Dictionary<string, string>, Project>("projects", new Dictionary<string, string>{
                {"ProjectName", projectName},
                {"Repo", githubRepo}
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
    }

    public sealed class ApiException(string message) : Exception(message)
    {
    }
}
