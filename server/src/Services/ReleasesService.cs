using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;

namespace ReleaseMonkey.Server.Services
{
    public class ReleasesService(ReleasessRepository releases)
    {
        public Task<Project> CreateRelease(string releaseName, int projectId)
        {
            return Task.FromResult(releases.InsertRelease(releaseName, projectId));
        }
    }
}