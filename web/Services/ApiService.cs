using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ReleaseMonkeyWeb.Models;

namespace ReleaseMonkeyWeb.Services
{
    public enum Build
    {
        Developer,
        Beta,
        Production,
    }

    public class ApiService(LocalPreferencesServices preferencesServices)
    {
        static readonly int BetaTesterId = 2;

        private readonly HttpClient http = new();
        private User? CurrentUser;

        public static Build CurrentBuild {get; set; } = Build.Developer;

        public async Task SetStorage()
        {
            CurrentUser = await preferencesServices.GetUser();
            if (CurrentUser != null)
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUser.Token);
            }
        }

        public void ConfigureEnv(string path)
        {
            if(path.Contains("localhost"))
            {
                CurrentBuild = Build.Developer;
            }
            else if(path.Contains(":8000"))
            {
                CurrentBuild = Build.Beta;
            }
            else
            {
                CurrentBuild = Build.Production;
            }
        }

        private static string BuildUrl(string path)
        {
            return CurrentBuild switch
            {
                Build.Developer => $"http://localhost:3000/{path}",
                Build.Beta => $"http://52.210.18.60:3000/{path}",
                _ => $"http://52.210.18.60:5000/{path}",
            };
        }

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
            var body = await Get<Dictionary<string, object>>($"release-testers/testers/{CurrentUser.Id}");
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

        public Task<PublicProject> GetPublicProject(string projectId)
        {
            return Get<PublicProject>($"projects/public/{projectId}");
        }

        public Task<UserProject> AddBetaTester(int projectId)
        {
            return Post<Dictionary<string, object>, UserProject>("user-projects/beta", new Dictionary<string, object>{
                {"UserId", CurrentUser!.Id},
                {"ProjectId", projectId},
                {"Role", BetaTesterId}
            });
        }
    }

    public sealed class ApiException(string message) : Exception(message)
    {
    }
}