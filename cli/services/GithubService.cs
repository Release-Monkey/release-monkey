using System.Diagnostics;
using System.Net;
using System.Web;

namespace ReleaseMonkey.Server.Services
{
    public class GithubService(string clientId, int callbackPort)
    {
        readonly string LocalServerUrl = $"http://localhost:{callbackPort}/";

        public Task<string> GetAccessCode()
        {
            OpenGithubLoginInBrowser();
            return Task.FromResult(WaitForAccessCode());
        }

        private void OpenGithubLoginInBrowser()
        {
            var url = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={LocalServerUrl}&scope=repo";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private string WaitForAccessCode()
        {
            HttpListener listener = new();
            listener.Prefixes.Add(LocalServerUrl);

            listener.Start();
            var context = listener.GetContext();

            var response = context.Response;
            var responseString = @"
                <!DOCTYPE html>
                <html>
                    <head>
                        <title>Welcome back</title>
                    </head>
                    <body>
                        <h1>Welcome back! You can close this window now.</h1>
                    </body>
                </html>";
            var responseBuffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentType = "text/html";
            response.ContentLength64 = responseBuffer.Length;
            response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
            response.OutputStream.Close();

            var request = context.Request;
            var queryValues = HttpUtility.ParseQueryString(request.Url!.Query);
            return queryValues.Get("code")!;
        }
    }
}