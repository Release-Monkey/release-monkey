using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ReleaseMonkeyWeb.Models;

namespace ReleaseMonkeyWeb.Services
{
    public class ApiService
    {
        private readonly HttpClient http = new();

        public ApiService(LocalPreferencesServices preferencesServices)
        {
            preferencesServices.GetUser().ContinueWith(async (getUserTask) =>
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

        public async Task<Requests.ReleaseTester>? UpdateRelease (Requests.ReleaseTester releaseTester)
        {
            var response = await http.PutAsJsonAsync<Requests.ReleaseTester>(BuildUrl("release-testers"), releaseTester);

            if (!response.IsSuccessStatusCode) return null;

            return releaseTester;
        }

        public async Task<List<Models.ReleaseTester>> FetchReleases ()
        {
            var body = await Get<Dictionary<string, object>>("release-testers");
            var releaseTesters = ((JsonElement) body["Result"]).Deserialize<List<Responses.ReleaseTester>>();
            body = await Get<Dictionary<string, object>>("releases");
            
            var releases = ((JsonElement) body["Result"]).Deserialize<List<Release>>();
            
            return releaseTesters!.Select<Responses.ReleaseTester, Models.ReleaseTester>(x => {
                var release = releases!.Find(y => y.Id == x.ReleaseId);

                return new Models.ReleaseTester(x.Id, release!, x.TesterId, x.State, x.Comment);
            }).ToList();
        }
        
        public List<Models.ReleaseTester> GetPendingReleases (List<Models.ReleaseTester> releaseTesters)
        {
            return releaseTesters.Where(x => x.State == 2).ToList();
        }
    }

    public sealed class ApiException(string message) : Exception(message)
    {
    }
}