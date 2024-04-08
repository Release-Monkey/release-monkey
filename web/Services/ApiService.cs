
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using ReleaseMonkeyWeb.Models;

namespace ReleaseMonkeyWeb.Services
{
    public class ApiService
    {
        private readonly HttpClient http = new();

        public ApiService(LocalPreferencesServices preferencesServices, ILocalStorageService localStorage)
        {
            preferencesServices.GetUser(localStorage).ContinueWith(async (getUserTask) =>
            {
                User? user = await getUserTask;
                if (user != null)
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                }
            });
        }

        private static string BuildUrl(string path) => $"http://localhost:3000/{path}";

        private async Task<R> Post<T, R>(string path, T body) where T : class where R : class
        {
            string jsonStr = JsonSerializer.Serialize(body);
            StringContent content = new(jsonStr, Encoding.UTF8, "application/json");

            var response = await http.PostAsync(BuildUrl(path), content);
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
            var response = await http.GetAsync(BuildUrl(path));
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

        public Task<User> LoginUser(string accessCode)
        {
            return Post<Dictionary<string, string>, User>("users", new Dictionary<string, string>{
                {"AccessCode", accessCode}
            });
        }
    }

    public sealed class ApiException(string message) : Exception(message)
    {
    }
}