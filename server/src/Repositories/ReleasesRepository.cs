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
            string sql = @"SELECT ReleaseID, ReleaseName, ProjectID, DownloadLink FROM [Release]";
            using SqlCommand command = new(sql, db.Connection);
            
            using SqlDataReader reader = db.ExecuteReader(command);
            
            while (reader.Read())
            {
                releases.Add(new Release(reader.GetInt32("ReleaseID"), reader.GetString("ReleaseName"), reader.GetInt32("ProjectID"), reader.GetString("DownloadLink")));
            }
            return releases;
        }

        public List<Release> GetReleasesByProjectId(Db db, int projectId)
        {
            List<Release> releases = new List<Release>();
            string sql = @"SELECT ReleaseID, ReleaseName, ProjectID, DownloadLink FROM [Release] WHERE ProjectID=@ProjectID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            while (reader.Read())
            {
                releases.Add(new Release(reader.GetInt32("ReleaseID"), reader.GetString("ReleaseName"), reader.GetInt32("ProjectID"), reader.GetString("DownloadLink")));
            }
            return releases;
        }

        public Release GetReleaseById(Db db, int releaseId)
        {
            string sql = @"SELECT ReleaseID, ReleaseName, ProjectID, DownloadLink FROM [Release] WHERE ReleaseID=@ReleaseID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ReleaseID", SqlDbType.Int).Value = releaseId;

            using SqlDataReader reader = db.ExecuteReader(command);

            if (reader.Read())
            {
                return new Release(reader.GetInt32("ReleaseID"), reader.GetString("ReleaseName"), reader.GetInt32("ProjectID"), reader.GetString("DownloadLink"));
            } else
            {
                throw new KeyNotFoundException($"There is no such release with id {releaseId}.");
            }
        }

        public Release InsertRelease(SqlTransaction transaction, Db db,string releaseName, int projectId, string downloadLink)
        {
            string sql = @"INSERT INTO [Release](ReleaseName, ProjectID, DownloadLink)
                            OUTPUT INSERTED.ReleaseID
                            VALUES(@ReleaseName, @ProjectID, @DownloadLink);";

            using SqlCommand command = new(sql, db.Connection, transaction);
            command.Parameters.Add("@ReleaseName", System.Data.SqlDbType.VarChar).Value = releaseName;
            command.Parameters.Add("@ProjectId", System.Data.SqlDbType.Int).Value = projectId;
            command.Parameters.Add("@DownloadLink", System.Data.SqlDbType.VarChar).Value = downloadLink;

            int releaseId = (int)command.ExecuteScalar();  
            return new Release(releaseId, releaseName, projectId, downloadLink);

        }
    }
}