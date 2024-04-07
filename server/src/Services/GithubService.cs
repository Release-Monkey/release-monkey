using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace ReleaseMonkey.Server.Services
{
    public class GithubService(string clientId, string clientSecret)
    {
        private readonly HttpClient client = new();

        public async Task<(string Name, string Email, string Token)> GetUser(string accessCode)
        {
            var requestBody = new Dictionary<string, string>{
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"code", accessCode}
            };

            string jsonStr = JsonSerializer.Serialize(requestBody);
            StringContent content = new(jsonStr, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://github.com/login/oauth/access_token", content);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();

            var token = stringResponse.Split('=')[1].Split('&')[0];
            var (Name, Email) = await GetUserInfo(token);

            return new(Name, Email, token);
        }

        public async Task<(string Name, string Email)> GetUserInfo(string accessToken)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, "https://api.github.com/user");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("release-monkey", "api"));
            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(stringResponse)!;

            requestMessage = new(HttpMethod.Get, "https://api.github.com/user/emails");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("release-monkey", "api"));
            response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            stringResponse = await response.Content.ReadAsStringAsync();
            var emailsResponse = JsonSerializer.Deserialize<Dictionary<string, object>[]>(stringResponse)!;

            var primaryEmail = emailsResponse.Where(item => bool.Parse(item["primary"].ToString()!) == true)
                .Select(item => item["email"]).FirstOrDefault();

            return (loginResponse["login"].ToString()!, primaryEmail!.ToString()!);
        }

        public async Task<IEnumerable<string>> ListRepos(string accessToken)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, "https://api.github.com/user/repos");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("release-monkey", "api"));
            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>[]>(stringResponse)!;

            return jsonResponse.Select(item => item["full_name"].ToString()!);
        }
    }
}
