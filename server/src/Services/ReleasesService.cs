using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;

namespace ReleaseMonkey.Server.Services
{
    public class ReleasesService(ReleasesRepository releases)
    {
        public Task<Release> CreateRelease(string releaseName, int projectId)
        {
            return Task.FromResult(releases.InsertRelease(releaseName, projectId));
        }
    }
}