using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ReleasesRepository(Db db)
    {
        public Release InsertRelease(SqlTransaction transaction, Db db,string releaseName, int projectId)
        {
            string sql = @"INSERT INTO [Release](ReleaseName, ProjectID)
                            OUTPUT INSERTED.ReleaseID
                            VALUES(@ReleaseName, @ProjectID);";

            using SqlCommand command = new(sql, db.Connection, transaction);
            command.Parameters.Add("@ReleaseName", System.Data.SqlDbType.VarChar).Value = releaseName;
            command.Parameters.Add("@ProjectId", System.Data.SqlDbType.Int).Value = projectId;

            int releaseId = (int)command.ExecuteScalar();  
            return new Release(releaseId, releaseName, projectId);

        }
    }
}