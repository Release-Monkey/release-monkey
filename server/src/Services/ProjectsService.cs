using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.Server.Types;

namespace ReleaseMonkey.Server.Services
{
    public class ProjectsService(ProjectsRepository projects)
    {
        public Task<Project> CreateProject(int userId, string projectName, string githubRepo, string token)
        {
            return Task.FromResult(projects.InsertProject(userId, projectName, githubRepo, token));
        }

        public Task<List<Project>> FetchProjects(Modifier modifier)
        {
            return Task.FromResult(projects.SelectProjects(modifier));
        }
    }
}