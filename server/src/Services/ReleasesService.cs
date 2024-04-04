using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.src.Repositories;
using System.Transactions;

namespace ReleaseMonkey.Server.Services
{
    public class ReleasesService(ReleasesRepository releases, Db db)
    {
        public Task<Release> CreateRelease(string releaseName, int projectId)
        {
            using (SqlTransaction transaction = db.Connection.BeginTransaction())
            {
                try
                {
                    Release release = releases.InsertRelease(transaction, db, releaseName, projectId);
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