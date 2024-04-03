using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Types;
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
            return new Project(reader.GetInt32("ProjectID"), projectName, githubRepo);
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