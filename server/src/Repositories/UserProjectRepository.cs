using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class UserProjectsRepository
    {
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
    }
}
