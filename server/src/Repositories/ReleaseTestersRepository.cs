using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ReleaseTestersRepository
    {
        public ReleaseTester InsertReleaseTester(SqlTransaction transaction, Db db, int releaseId, int testerId)
        {
            string sql = @" INSERT INTO[ReleaseTester](ReleaseId, TesterId, State, Comment)
                            VALUES(@ReleaseId, @TesterId, @State, @Comment);
                            SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new(sql, db.Connection, transaction))
            {
                command.Parameters.Add("@ReleaseId", System.Data.SqlDbType.Int).Value = releaseId;
                command.Parameters.Add("@TesterId", System.Data.SqlDbType.Int).Value = testerId;
                command.Parameters.Add("@State", System.Data.SqlDbType.Int).Value = 2;
                command.Parameters.Add("@Comment", System.Data.SqlDbType.VarChar).Value = "";
                
                int ReleaseTesterId = (int) (decimal) command.ExecuteScalar();
                return new ReleaseTester(ReleaseTesterId, releaseId, testerId, 2, "");
                
            }
        }
    }
}