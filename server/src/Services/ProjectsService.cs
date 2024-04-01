using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;

namespace ReleaseMonkey.Server.Services
{
    public class ProjectsService(ProjectsRepository projects)
    {
        public Task<Project> CreateProject(int userId, string projectName, string githubRepo, string token)
        {
            return Task.FromResult(projects.InsertProject(userId, projectName, githubRepo, token));
        }
    }
}