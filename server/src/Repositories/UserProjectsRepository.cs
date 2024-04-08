using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class UserProjectsRepository
    {
        public List<UserProject> GetTestersForProject(SqlTransaction transaction,Db db, int projectId)
        {
            List<UserProject> userProjects = new List<UserProject>();
            string sql = @"SELECT UserProjectID, UserID, ProjectID, Role FROM [UserProject] WHERE ProjectID=@ProjectID AND (Role=1 OR Role=2)";
            using SqlCommand command = new(sql, db.Connection, transaction);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            while (reader.Read())
            {
                userProjects.Add(new UserProject(reader.GetInt32("UserProjectID"), reader.GetInt32("UserID"), reader.GetInt32("ProjectID"), reader.GetInt32("Role")));
            }
            return userProjects;
        }

        public List<UserProject> GetTestersForProject(Db db, int projectId)
        {
            List<UserProject> userProjects = new List<UserProject>();
            string sql = @"SELECT UserProjectID, UserID, ProjectID, Role FROM [UserProject] WHERE ProjectID=@ProjectID AND (Role=1 OR Role=2)";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            while (reader.Read())
            {
                userProjects.Add(new UserProject(reader.GetInt32("UserProjectID"), reader.GetInt32("UserID"), reader.GetInt32("ProjectID"), reader.GetInt32("Role")));
            }
            return userProjects;
        }

        public List<UserProject> GetUsersForProject(Db db, int projectId)
        {
            List<UserProject> userProjects = new List<UserProject>();
            string sql = @"SELECT UserProjectID, UserID, ProjectID, Role FROM [UserProject] WHERE ProjectID=@ProjectID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            while (reader.Read())
            {
                userProjects.Add(new UserProject(reader.GetInt32("UserProjectID"), reader.GetInt32("UserID"), reader.GetInt32("ProjectID"), reader.GetInt32("Role")));
            }
            return userProjects;
        }

        public UserProject GetUserProjectById(Db db, int userProjectId)
        {
            string sql = @"SELECT UserProjectID, UserID, ProjectID, Role FROM [UserProject] WHERE UserProjectID=@UserProjectID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@UserProjectID", SqlDbType.Int).Value = userProjectId;

            using SqlDataReader reader = db.ExecuteReader(command);

            if (reader.Read())
            {
                return new UserProject(reader.GetInt32("UserProjectID"), reader.GetInt32("UserID"), reader.GetInt32("ProjectID"), reader.GetInt32("Role"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such user project with id {userProjectId}.");
            }
        }

        public UserProject InsertUserProject(SqlTransaction transaction, Db db, int userId, int projectID, int role)
        {
            string sql = @" INSERT INTO[UserProject](UserId, ProjectID, Role)
                            VALUES(@UserId, @ProjectID, @Role);
                            SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new(sql, db.Connection, transaction))
            {
                command.Parameters.Add("@UserId", System.Data.SqlDbType.Int).Value = userId;
                command.Parameters.Add("@Role", System.Data.SqlDbType.Int).Value = role;
                command.Parameters.Add("@ProjectID", System.Data.SqlDbType.Int).Value = projectID;
                
                int userProjectId = (int) (decimal) command.ExecuteScalar();
                return new UserProject(userProjectId, userId, projectID, role);
                
            }
        }

        public List<int> GetUserIdsWithRole(Db db, int role, int projectId)
        {
            string sql = @"SELECT UserID FROM UserProject
                            WHERE Role=@Role AND ProjectID=@ProjectID";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@Role", SqlDbType.Int).Value = role;
            command.Parameters.Add("@ProjectID", SqlDbType.Int).Value = projectId;

            List<int> output = [];
            using SqlDataReader reader = db.ExecuteReader(command);
            while(reader.Read())
            {
                output.Add(reader.GetInt32("UserID"));
            }

            return output;
        }

    }
}

