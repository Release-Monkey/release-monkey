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
                            VALUES(@ProjectName, @Repo, @Token);";

            using (SqlCommand command = new(sql, db.Connection, transaction))
            {
                command.Parameters.Add("@ProjectName", System.Data.SqlDbType.VarChar).Value = projectName;
                command.Parameters.Add("@Repo", System.Data.SqlDbType.VarChar).Value = repo;
                command.Parameters.Add("@Token", System.Data.SqlDbType.VarChar).Value = token;

                if (db == null || db.Connection == null || transaction == null)
                {
                    Console.WriteLine("Null");// Handle the null condition appropriately, such as throwing an exception or logging an error.
                }

                int projectId = (int)command.ExecuteScalar();
                return new Project(projectId, projectName, repo);

            }
        }
    }
}

