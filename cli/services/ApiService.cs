using cli.models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace cli.services
{
    internal class ApiService(AuthService AuthService)
    {

        private readonly HttpClient httpClient = new();

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

        public Task<Project> CreateProject(string projectName, string githubRepo)
        {
            return Post<Dictionary<string, string>, Project>("projects", new Dictionary<string, string>{
                {"Name", projectName},
                {"Repository", githubRepo}
            });
        }
    }

    public sealed class ApiException(string message) : Exception(message)
    {
    }
}
