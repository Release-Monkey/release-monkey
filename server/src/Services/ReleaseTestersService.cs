using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.src.Common;
using ReleaseMonkey.src.Repositories;
using System.Transactions;

namespace ReleaseMonkey.Server.Services
{
    public class ReleaseTestersService(ReleaseTestersRepository releaseTesters, Db db)
    {
        public Task<List<ReleaseTester>> GetAllReleaseTesters()
        {
            return Task.FromResult(releaseTesters.GetAllReleaseTesters(db));
        }

        public Task<List<ReleaseTester>> GetReleaseTestersByReleaseId(int releaseId)
        {
            return Task.FromResult(releaseTesters.GetReleaseTestersByReleaseId(db, releaseId));
        }

        public Task<ReleaseTester> GetReleaseTesterById(int releaseTesterId)
        {
            return Task.FromResult(releaseTesters.GetReleaseTesterById(db, releaseTesterId));
        }

        public Task<ReleaseTester> UpdateReleaseTester(int ReleaseTesterId, int State, string Comment)
        {
            ReleaseTester releaseTester = releaseTesters.UpdateReleaseTester(db, ReleaseTesterId, State, Comment);
            return Task.FromResult(releaseTester);

        }

        public Task<ReleaseTester> CreateReleaseTester(int ReleaseId, int TesterId)
        {
            ReleaseTester releaseTester = releaseTesters.InsertReleaseTester(db, ReleaseId, TesterId);
            return Task.FromResult(releaseTester);
        }
    }
}
