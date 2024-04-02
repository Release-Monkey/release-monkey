using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ReleasesRepository(Db db)
    {
        public Release InsertRelease(string releaseName, int projectId)
        {
            string sql = @"INSERT INTO [Release](ReleaseName, ProjectID)
                            OUTPUT INSERTED.ReleaseID
                            VALUES(@ReleaseName, @ProjectID);";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ReleaseName", System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@ProjectId", System.Data.SqlDbType.Int);

            command.Parameters["@ReleaseName"].Value = releaseName;
            command.Parameters["@ProjectId"].Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);
            reader.Read();
            return new Release(reader.GetInt32("ReleaseID"), releaseName, projectId);
        }
    }
}