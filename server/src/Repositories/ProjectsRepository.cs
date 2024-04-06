using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Types;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ProjectsRepository
    {
        public Project GetProjecById(Db db, int projectId)
        {
            string sql = @"SELECT ProjectID, ProjectName, Repo, Token FROM [Project] WHERE ProjectID=@ProjectID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            if (reader.Read())
            {
                return new Project(projectId, reader.GetString("ProjectName"), reader.GetString("Repo"), reader.GetString("Token"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such release with id {projectId}.");
            }
        }

        public Project GetProjectById(SqlTransaction transaction, Db db, int projectId)
        {
            string sql = @"SELECT ProjectID, ProjectName, Repo, Token FROM [Project] WHERE ProjectID=@ProjectID";
            using SqlCommand command = new(sql, db.Connection, transaction);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            if (reader.Read())
            {
                return new Project(projectId, reader.GetString("ProjectName"), reader.GetString("Repo"), reader.GetString("Token"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such release with id {projectId}.");
            }
        }

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
                return new Project(projectId, projectName, repo, token);

            }
        }

        public Project GetProjectById(Db db, int projectId)
        {
            string sql = @"SELECT ProjectName, Repo FROM [Project]
                            WHERE ProjectID=@ProjectID";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);
            if (reader.Read())
            {
                return new Project(projectId, reader.GetString("ProjectName"), reader.GetString("Repo"), reader.GetString("Token"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such project with id {projectId}.");
            }
        }

        public List<Project> SelectProjects(Modifier modifier)
        {
            List<Project> projects = [];

            string sql = @"
                SELECT ProjectID, ProjectName, Repo
                FROM [Project]
                WHERE ProjectName LIKE '%' + @SearchTerm + '%'
                ORDER BY
                    (CASE WHEN @OrderBy = 'id' AND @SortDirection = 'ASC' THEN ProjectID END) ASC,
                    (CASE WHEN @OrderBy = 'name' AND @SortDirection = 'ASC' THEN ProjectName END) ASC,
                    (CASE WHEN @OrderBy = 'repo' AND @SortDirection = 'ASC' THEN Repo END) ASC,
                    (CASE WHEN @OrderBy = 'id' AND @SortDirection = 'DESC' THEN ProjectID END) DESC,
                    (CASE WHEN @OrderBy = 'name' AND @SortDirection = 'DESC' THEN ProjectName END) DESC,
                    (CASE WHEN @OrderBy = 'repo' AND @SortDirection = 'DESC' THEN Repo END) DESC
                OFFSET (@Page - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY;
            ";

            using (SqlCommand command = new (sql, db.Connection))
            {
                command.Parameters.AddWithValue("@SearchTerm", modifier.searchTerm);
                command.Parameters.AddWithValue("@OrderBy", modifier.orderBy);
                command.Parameters.AddWithValue("@SortDirection", modifier.sort);
                command.Parameters.AddWithValue("@Page", modifier.page);
                command.Parameters.AddWithValue("@PageSize", modifier.size);

                using (SqlDataReader reader = db.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        var project = new Project(
                            reader.GetInt32("ProjectID"),
                            reader.GetString("ProjectName"),
                            reader.GetString("Repo")
                        );
                        projects.Add(project);
                    }
                }
            }

            return projects;
        }
    }
}

