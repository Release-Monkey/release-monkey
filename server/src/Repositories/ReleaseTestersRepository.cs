using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class ReleaseTestersRepository
    {
        public List<ReleaseTester> GetAllReleaseTesters(Db db)
        {
            List<ReleaseTester> releaseTesters = new List<ReleaseTester>();
            string sql = @"SELECT ReleaseTesterID, ReleaseID, TesterID, State, Comment FROM [ReleaseTester]";
            using SqlCommand command = new(sql, db.Connection);

            using SqlDataReader reader = db.ExecuteReader(command);

            while (reader.Read())
            {
                releaseTesters.Add(new ReleaseTester(reader.GetInt32("ReleaseTesterID"), reader.GetInt32("ReleaseID"), reader.GetInt32("TesterID"), reader.GetInt32("State"), reader.GetString("Comment")));
            }
            return releaseTesters;
        }

        public List<ReleaseTester> GetReleaseTestersByReleaseId(Db db, int releaseId)
        {
            List<ReleaseTester> releaseTesters = new List<ReleaseTester>();
            string sql = @"SELECT ReleaseTesterID, ReleaseID, TesterID, State, Comment FROM [ReleaseTester] WHERE ReleaseID=@ReleaseID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ReleaseID", SqlDbType.Int).Value = releaseId;

            using SqlDataReader reader = db.ExecuteReader(command);

            while (reader.Read())
            {
                releaseTesters.Add(new ReleaseTester(reader.GetInt32("ReleaseTesterID"), reader.GetInt32("ReleaseID"), reader.GetInt32("TesterID"), reader.GetInt32("State"), reader.GetString("Comment")));
            }
            return releaseTesters;
        }

        public ReleaseTester GetReleaseTesterById(Db db, int releaseTesterId)
        {
            string sql = @"SELECT ReleaseTesterID, ReleaseID, TesterID, State, Comment FROM [ReleaseTester] WHERE ReleaseTesterID=@ReleaseTesterID";
            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@ReleaseTesterID", SqlDbType.Int).Value = releaseTesterId;

            using SqlDataReader reader = db.ExecuteReader(command);

            if (reader.Read())
            {
                return new ReleaseTester(reader.GetInt32("ReleaseTesterID"), reader.GetInt32("ReleaseID"), reader.GetInt32("TesterID"), reader.GetInt32("State"), reader.GetString("Comment"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such release with id {releaseTesterId}.");
            }
        }

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

        public ReleaseTester InsertReleaseTester(Db db, int releaseId, int testerId)
        {
            string sql = @" INSERT INTO[ReleaseTester](ReleaseId, TesterId, State, Comment)
                            VALUES(@ReleaseId, @TesterId, @State, @Comment);
                            SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new(sql, db.Connection))
            {
                command.Parameters.Add("@ReleaseId", System.Data.SqlDbType.Int).Value = releaseId;
                command.Parameters.Add("@TesterId", System.Data.SqlDbType.Int).Value = testerId;
                command.Parameters.Add("@State", System.Data.SqlDbType.Int).Value = 2;
                command.Parameters.Add("@Comment", System.Data.SqlDbType.VarChar).Value = "";

                int ReleaseTesterId = (int)(decimal)command.ExecuteScalar();
                return new ReleaseTester(ReleaseTesterId, releaseId, testerId, 2, "");

            }
        }

        public ReleaseTester UpdateReleaseTester(Db db, int releaseTesterId, int state, string comment)
        {
            string sql = @"UPDATE [ReleaseTester] SET State=@State, Comment=@Comment OUTPUT INSERTED.ReleaseTesterID, INSERTED.ReleaseID, INSERTED.TesterID, INSERTED.State, INSERTED.Comment WHERE ReleaseTesterId=@ReleaseTesterId;";

            using (SqlCommand command = new(sql, db.Connection))
            {
                command.Parameters.Add("@ReleaseTesterId", System.Data.SqlDbType.Int).Value = releaseTesterId;
                command.Parameters.Add("@State", System.Data.SqlDbType.Int).Value = state;
                command.Parameters.Add("@Comment", System.Data.SqlDbType.VarChar).Value = comment;

                using SqlDataReader reader = db.ExecuteReader(command);

                if (reader.Read())
                {
                    Console.WriteLine(reader.GetInt32("ReleaseID"));
                    return new ReleaseTester(releaseTesterId, reader.GetInt32("ReleaseID"), reader.GetInt32("TesterID"), reader.GetInt32("State"), reader.GetString("Comment"));
                }
                else
                {
                    if (state == 0) throw new KeyNotFoundException($"Approval failed due to key error");
                    else throw new KeyNotFoundException("Rejection failed due to key error");
                }

            }
        }
    }
}