using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ReleasesRepository
    {
        public List<Release> GetAllReleases(Db db)
        {
            List<Release> releases = new List<Release>();
            string sql = @"SELECT * FROM [Release]";
            using SqlCommand command = new(sql, db.Connection);
            
            using SqlDataReader reader = db.ExecuteReader(command);
            
            while (reader.Read())
            {
                releases.Add(new Release(reader.GetInt32("ReleaseID"), reader.GetString("ReleaseName"), reader.GetInt32("ProjectID")));
            }
            return releases;
        }

        public List<Release> GetReleasesByProjectId(Db db, int projectId)
        {
            List<Release> releases = new List<Release>();
            string sql = @"SELECT * FROM [Release] WHERE ProjectID=@ProjectID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            while (reader.Read())
            {
                releases.Add(new Release(reader.GetInt32("ReleaseID"), reader.GetString("ReleaseName"), reader.GetInt32("ProjectID")));
            }
            return releases;
        }

        public Release GetReleasesById(Db db, int releaseId)
        {
            string sql = @"SELECT * FROM [Release] WHERE ReleaseID=@ReleaseID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ReleaseID", SqlDbType.Int).Value = releaseId;

            using SqlDataReader reader = db.ExecuteReader(command);

            if (reader.Read())
            {
                return new Release(releaseId, reader.GetString("ReleaseName"), reader.GetInt32("ProjectID"));
            } else
            {
                throw new KeyNotFoundException($"There is no such release with id {releaseId}.");
            }
        }

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