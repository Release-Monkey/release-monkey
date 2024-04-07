using Microsoft.AspNetCore.WebUtilities;
using Blazored.LocalStorage;

namespace ReleaseMonkeyWeb.Repository
{
    public class User
    {
        private readonly HttpClient http;
        private readonly ILocalStorageService localStorage;

        public User (HttpClient http, ILocalStorageService localStorage)
        {
            this.http = http;
            this.localStorage = localStorage;
        }

        public async Task <string> FetchAccessToken (string accessCode)
        {
            var clientId = Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("GITHUB_CLIENT_SECRET");

            var url = "https://github.com/login/oauth/access_token";

            var payload = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "client_id", $"{clientId}" },
                { "client_secret", $"{clientSecret}" },
                { "code", accessCode },
            });

            var response = await http.PostAsync(url, payload);
            var content = await response.Content.ReadAsStringAsync();
            var parameters = QueryHelpers.ParseQuery(content);

            return parameters["access_token"];
        }

        public async Task CacheAccessToken (string token)
        {
            await localStorage.SetItemAsync("GITHUB_API_ACCESS_TOKEN", token);
        }

        public async Task <string> GetAccessToken ()
        {
            return await localStorage.GetItemAsync<string>("GITHUB_API_ACCESS_TOKEN") ?? "";
        }
    }
}
