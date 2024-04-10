using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ReleaseMonkey.Server.Services
{
    public record CreateTagRequest
    (
        string tag,
        string message,
        string gitObject,
        string type,
        Tagger tagger
    );

    public record CreateReleaseRequest
    (
        string tagName,
        string message,
        string targetCommitish,
        string name,
        string body,
        bool draft,
        bool prerelease
    );

    public record Tagger(string name, string email, string date);
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

        public async void ReleaseProject(string repo, string accessToken, string releaseName)
        {
            try 
            {
            HttpRequestMessage request1Message = new(HttpMethod.Get, "https://api.github.com/repos/"+repo+"/branches/master");
            request1Message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request1Message.Headers.UserAgent.Add(new ProductInfoHeaderValue("release-monkey", "api"));
            var response1 = await client.SendAsync(request1Message);
            response1.EnsureSuccessStatusCode();

            var stringResponse = await response1.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(stringResponse)!;
            var commit = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse["commit"].ToString())!;

                try
                {
                HttpRequestMessage requestMessage = new(HttpMethod.Post, "https://api.github.com/repos/"+repo+"/git/tags");
                var tagger = GetUserInfo(accessToken);
                var requestBody = new CreateTagRequest(releaseName.Replace(" ","_"), releaseName, commit["sha"].ToString(), "commit", new Tagger(tagger.Result.Name, tagger.Result.Email, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")));
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("release-monkey", "api"));
                StringContent content = new(JsonSerializer.Serialize(requestBody).Replace("gitObject","object"), Encoding.UTF8, "application/json");
                requestMessage.Content = content;
                var response = await client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                    try 
                    {
                        HttpRequestMessage releaseRequestMessage = new(HttpMethod.Post, "https://api.github.com/repos/" + repo + "/releases");
                        var releaseRequestBody = new CreateReleaseRequest(releaseName.Replace(" ", "_"), releaseName, commit["sha"].ToString(), releaseName, "Automatic release "+releaseName+" generated by Release Monkey", false, false);
                        releaseRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        releaseRequestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("release-monkey", "api"));
                        StringContent releaseContent = new(JsonSerializer.Serialize(releaseRequestBody).Replace("targetCommitish","target_commitish").Replace("tagName","tag_name"), Encoding.UTF8, "application/json");
                        releaseRequestMessage.Content = releaseContent;
                        var releaseResponse = await client.SendAsync(releaseRequestMessage);
                        releaseResponse.EnsureSuccessStatusCode();
                    } catch (Exception e){
                        Console.WriteLine("Release Request Error: ",e.Message);
                    }

                }
                catch (Exception e){
                    Console.WriteLine("Tag Request Error: ",e.Message);
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine("Branch Request Error: ",e.Message);
            }
        }
    }
}
