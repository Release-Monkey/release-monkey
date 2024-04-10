using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ProjectsRepository
    {
        public List<Project> GetProjects(Db db)
        {
            string sql = @"SELECT ProjectID, ProjectName, Repo, Token, PublicProject FROM [Project]";
            using SqlCommand command = new(sql, db.Connection);

            using SqlDataReader reader = db.ExecuteReader(command);

            List<Project> result = new List<Project>();

            while (reader.Read())
            {
                result.Add(new Project(reader.GetInt32("ProjectID"), reader.GetString("ProjectName"), reader.GetString("Repo"), reader.GetString("Token"), reader.GetBoolean("PublicProject")));
            }
            return result;
        }

        public Project GetProjecById(Db db, int projectId)
        {
            string sql = @"SELECT ProjectID, ProjectName, Repo, Token, PublicProject FROM [Project] WHERE ProjectID=@ProjectID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            if (reader.Read())
            {
                return new Project(projectId, reader.GetString("ProjectName"), reader.GetString("Repo"), reader.GetString("Token"), reader.GetBoolean("PublicProject"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such release with id {projectId}.");
            }
        }

        public Project GetProjectById(SqlTransaction transaction, Db db, int projectId)
        {
            string sql = @"SELECT ProjectID, ProjectName, Repo, Token, PublicProject FROM [Project] WHERE ProjectID=@ProjectID";
            using SqlCommand command = new(sql, db.Connection, transaction);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            if (reader.Read())
            {
                return new Project(projectId, reader.GetString("ProjectName"), reader.GetString("Repo"), reader.GetString("Token"), reader.GetBoolean("PublicProject"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such release with id {projectId}.");
            }
        }

        public Project InsertProject(SqlTransaction transaction, Db db, string projectName, string repo, string token, bool publicProject)
        {
            string sql = @"INSERT INTO [Project](ProjectName, Repo, Token, PublicProject)
                            VALUES(@ProjectName, @Repo, @Token, @PublicProject);
                            SELECT SCOPE_IDENTITY();"; // SCOPE_IDENTITY() returns the last identity value generated in the current session

            using (SqlCommand command = new(sql, db.Connection, transaction))
            {
                command.Parameters.Add("@ProjectName", System.Data.SqlDbType.VarChar).Value = projectName;
                command.Parameters.Add("@Repo", System.Data.SqlDbType.VarChar).Value = repo;
                command.Parameters.Add("@Token", System.Data.SqlDbType.VarChar).Value = token;
                command.Parameters.Add("@PublicProject", System.Data.SqlDbType.Bit).Value = publicProject;

                int projectId = (int) (decimal) command.ExecuteScalar(); // Make use of ExecutionReader if you want to retrieve more than one value ExecuteScalar() is only retrieving the first column of the first row    
                return new Project(projectId, projectName, repo, token, publicProject);

            }
        }

        public Project UpdateProject(Db db, int projectId, string projectName, string repo, string token, bool publicProject)
        {
            string sql = @"UPDATE [Project] SET ProjectName=@ProjectName, Repo=@Repo, Token=@Token, PublicProject=@PublicProject OUTPUT INSERTED.ProjectID, INSERTED.ProjectName, INSERTED.Repo, INSERTED.Token, INSERTED.PublicProject WHERE ProjectID=@ProjectID;";

            using (SqlCommand command = new(sql, db.Connection))
            {
                command.Parameters.Add("@ProjectID", System.Data.SqlDbType.Int).Value = projectId;
                command.Parameters.Add("@ProjectName", System.Data.SqlDbType.VarChar).Value = projectName;
                command.Parameters.Add("@Repo", System.Data.SqlDbType.VarChar).Value = repo;
                command.Parameters.Add("@Token", System.Data.SqlDbType.VarChar).Value = token;
                command.Parameters.Add("@PublicProject", System.Data.SqlDbType.Bit).Value = publicProject;
                using SqlDataReader reader = db.ExecuteReader(command);

                if (reader.Read())
                {
                    return new Project(reader.GetInt32("ProjectID"), reader.GetString("ProjectName"), reader.GetString("Repo"), reader.GetString("Token"), reader.GetBoolean("PublicProject"));
                }
                else
                {
                    throw new KeyNotFoundException($"Update failed");
                }
            }
        }

        public Project GetProjectById(Db db, int projectId)
        {
            string sql = @"SELECT ProjectName, Repo, Token, PublicProject FROM [Project]
                            WHERE ProjectID=@ProjectID";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);
            if (reader.Read())
            {
                return new Project(projectId, reader.GetString("ProjectName"), reader.GetString("Repo"), reader.GetString("Token"), reader.GetBoolean("PublicProject"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such project with id {projectId}.");
            }
        }

        public PublicProject GetPublicProjectById(Db db, int projectId)
        {
            string sql = @"SELECT ProjectName FROM [Project]
                            WHERE ProjectID=@ProjectID AND PublicProject=1";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);
            if (reader.Read())
            {
                return new PublicProject(projectId, reader.GetString("ProjectName"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no publicly available project with id {projectId}.");
            }
        }

        public List<PublicProject> GetPublicProjects(Db db)
        {
            string sql = @"SELECT ProjectID, ProjectName FROM [Project]
                            WHERE PublicProject=1";

            using SqlCommand command = new(sql, db.Connection);

            using SqlDataReader reader = db.ExecuteReader(command);

            List<PublicProject> list = new List<PublicProject>();
            while (reader.Read())
            {
                list.Add(new PublicProject(reader.GetInt32("ProjectID"), reader.GetString("ProjectName")));
            }

            return list;

        }
    }
}

