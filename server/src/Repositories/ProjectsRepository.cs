using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ProjectsRepository
    {
        public Project InsertProject(SqlTransaction transaction, Db db, string projectName, string repo, string token)
        {
            string sql = @"INSERT INTO [Project](ProjectName, Repo, Token)
                            VALUES(@ProjectName, @Repo, @Token);
                            SELECT SCOPE_IDENTITY();"; // SCOPE_IDENTITY() returns the last identity value generated in the current session

            using (SqlCommand command = new(sql, db.Connection, transaction))
            {
                command.Parameters.Add("@ProjectName", System.Data.SqlDbType.VarChar).Value = projectName;
                command.Parameters.Add("@Repo", System.Data.SqlDbType.VarChar).Value = repo;
                command.Parameters.Add("@Token", System.Data.SqlDbType.VarChar).Value = token;

                int projectId = (int) (decimal) command.ExecuteScalar(); // Make use of ExecutionReader if you want to retrieve more than one value ExecuteScalar() is only retrieving the first column of the first row    
                return new Project(projectId, projectName, repo);

            }
        }
    }
}

