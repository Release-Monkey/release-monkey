using cli.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli
{
    internal class Commands(AuthService authService, ApiService apiService)
    {
        public Task LoginWithGithub() { throw new NotImplementedException(); }

        public Task Logout() { throw new NotImplementedException(); }

        public async Task CreateProject(string projectName, string githubRepo)
        {
            try
            {
                var project = await apiService.CreateProject(projectName, githubRepo);
                Console.WriteLine($"Project has been created. Project id is {project.Id}");
            }
            catch (ApiException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Task SetProject(string projectId) { throw new NotImplementedException(); }

        public Task ListProjects() { throw new NotImplementedException(); }

        public Task AddTesters(List<string> testerEmails) { throw new NotImplementedException(); }

        public Task CreateRelease(string releaseName) { throw new NotImplementedException(); }

        public Task ListReleases() { throw new NotImplementedException(); }

        public Task ApproveRelease(string releaseId) { throw new NotImplementedException(); }

        public void PrintHelp()
        {
            Console.WriteLine("Welcome to Release Monkey. Use CLI to do everything.");
        }
    }
}
