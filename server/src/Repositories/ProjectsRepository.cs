using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ProjectsRepository(Db db)
    {
        public Project InsertProject(int userId, string projectName, string githubRepo, string token)
        {
            string sql = @"INSERT INTO [Project](ProjectName, Repo, Token)
                            VALUES(@ProjectName, @Repo, @Token);
                           INSERT INTO[UserProject](UserId, ProjectID, Role)
                            OUTPUT INSERTED.ProjectID
                            VALUES(@UserId, SCOPE_IDENTITY(), @Role)";
            using (SqlTransaction transaction = db.Connection.BeginTransaction())
            using (SqlCommand command = new(sql, db.Connection, transaction))
            {
                command.Parameters.Add("@ProjectName", System.Data.SqlDbType.VarChar).Value = projectName;
                command.Parameters.Add("@Repo", System.Data.SqlDbType.VarChar).Value = githubRepo;
                command.Parameters.Add("@Token", System.Data.SqlDbType.VarChar).Value = token;
                command.Parameters.Add("@UserId", System.Data.SqlDbType.Int).Value = userId;
                command.Parameters.Add("@Role", System.Data.SqlDbType.Int).Value = 1;

                try
                {
                    int projectId = (int)command.ExecuteScalar();
                    transaction.Commit();

                    return new Project(projectId, projectName, githubRepo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();
                    return new Project(-1, string.Empty, string.Empty);
                }
            }
        }
    }
}

