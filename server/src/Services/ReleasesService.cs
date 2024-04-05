using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.src.Repositories;
using System.Transactions;

namespace ReleaseMonkey.Server.Services
{
    public class ReleasesService(ReleasesRepository releases, ReleaseTestersRepository releaseTesters, UserProjectsRepository userProjects, Db db)
    {
        public Task<List<Release>> GetAllReleases() 
        {
            return Task.FromResult(releases.GetAllReleases(db));
        }

        public Task<List<Release>> GetReleaseByProjectId(int projectId)
        {
            return Task.FromResult(releases.GetReleasesByProjectId(db, projectId));
        }

        public Task<Release> GetReleaseById(int releaseId)
        {
            return Task.FromResult(releases.GetReleasesById(db, releaseId));
        }

        public Task<Release> CreateRelease(string releaseName, int projectId)
        {
            using (SqlTransaction transaction = db.Connection.BeginTransaction())
            {
                try
                {
                    Release release = releases.InsertRelease(transaction, db, releaseName, projectId);
                    List<UserProject> userProjectList = userProjects.GetTestersForProject(transaction, db, projectId);
                    List<UserProject> testerList = userProjectList.Where(u => u.Role == 1).ToList();
                    foreach (UserProject userProject in testerList)
                    {
                        releaseTesters.InsertReleaseTester(transaction, db, release.Id, userProject.UserId);
                    }

                    transaction.Commit();
                    return Task.FromResult(release);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            
        }
    }
}