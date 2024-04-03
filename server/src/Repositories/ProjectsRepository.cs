using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ProjectsRepository(Db db)
    {
        public Project InsertProject(int userId, string projectName, string githubRepo, string token)
        {
            string sql = @"INSERT INTO [Project](ProjectName, Repo, Token)
                            VALUES(@ProjectName, @Repo, @Token);
                          INSERT INTO [UserProject](UserId, ProjectID, Role)
                            OUTPUT INSERTED.ProjectID
                            VALUES(@UserId, SCOPE_IDENTITY(), @Role)";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectName", System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@Repo", System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@Token", System.Data.SqlDbType.VarChar);
            command.Parameters.Add("@UserId", System.Data.SqlDbType.Int);
            command.Parameters.Add("@Role", System.Data.SqlDbType.Int);

            command.Parameters["@ProjectName"].Value = projectName;
            command.Parameters["@Repo"].Value = githubRepo;
            command.Parameters["@Token"].Value = token;
            command.Parameters["@UserId"].Value = userId;
            command.Parameters["@Role"].Value = 1;

            using SqlDataReader reader = db.ExecuteReader(command);
            reader.Read();
            return new Project(reader.GetInt32("ProjectID"), projectName, githubRepo, token, userId);
        }
    }
}