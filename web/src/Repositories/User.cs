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
            var clientId = "527ca76c9121b85da825";
            var clientSecret = "4e46f318b446c424346d55b7e7570f9601e27c6b";

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
