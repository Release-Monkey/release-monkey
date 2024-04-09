using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.src.Repositories;
using System.Data;

namespace ReleaseMonkey.Server.Repositories
{
    public class UsersRepository(Db db)
    {
        public User InsertOrUpdateUser(string Name, string Email)
        {
            string sql = @"IF EXISTS(SELECT UserID FROM [User] WHERE EmailAddress=LOWER(@EmailAddress))
                            BEGIN
                                UPDATE [User]
                                SET Name=@Name
                                OUTPUT INSERTED.UserID
                                WHERE EmailAddress=@EmailAddress
                            END
                            ELSE
                            BEGIN
                                INSERT INTO [User](Name, EmailAddress)
                                OUTPUT INSERTED.UserID
                                VALUES(@Name, LOWER(@EmailAddress))
                            END";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@Name", SqlDbType.VarChar);
            command.Parameters.Add("@EmailAddress", SqlDbType.VarChar);

            command.Parameters["@Name"].Value = Name;
            command.Parameters["@EmailAddress"].Value = Email;

            using SqlDataReader reader = db.ExecuteReader(command);
            reader.Read();
            return new User(reader.GetInt32("UserID"), Name, Email);
        }

        public User GetUserById(int userId)
        {
            string sql = @"SELECT UserID, Name, EmailAddress
                           FROM [User] WHERE UserID=@UserID";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@UserID", SqlDbType.Int);
            command.Parameters["@UserID"].Value = userId;

            using SqlDataReader reader = db.ExecuteReader(command);
            if (reader.Read())
            {
                return new User(reader.GetInt32("UserID"), reader.GetString("Name"), reader.GetString("EmailAddress"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such user with id {userId}.");
            }
        }

        public User FindByEmail(string emailAddress)
        {
            string sql = @"SELECT UserID, Name, EmailAddress
                           FROM [User] WHERE EmailAddress=@EmailAddress";

            using SqlCommand command = new(sql, db.Connection);
            command.Parameters.Add("@EmailAddress", SqlDbType.VarChar);
            command.Parameters["@EmailAddress"].Value = emailAddress;

            using SqlDataReader reader = db.ExecuteReader(command);
            if (reader.Read())
            {
                return new User(reader.GetInt32("UserID"), reader.GetString("Name"), reader.GetString("EmailAddress"));
            }
            else
            {
                throw new KeyNotFoundException($"There is no such user with email {emailAddress}.");
            }
        }

        public List<string> GetUserEmailsByIds(SqlTransaction transaction, Db db, List<int> userIds)
        {
            if (userIds.Count == 0)
            {
                return [];
            }
            else
            {
                string sql = @"SELECT EmailAddress
                           FROM [User] WHERE UserID IN ({0})";

                using SqlCommand command = new(sql, db.Connection, transaction);

                var idParameterList = new List<string>(userIds.Count);
                for (int index = 0; index < userIds.Count; index++)
                {
                    idParameterList[index] = $"@UserId{index}";
                    command.Parameters.Add(idParameterList[index], SqlDbType.Int).Value = userIds[index];
                }
                command.CommandText = string.Format(sql, string.Join(",", idParameterList));
                using SqlDataReader reader = db.ExecuteReader(command);

                List<string> emails = [];
                while (reader.Read())
                {
                    emails.Add(reader.GetString("EmailAddress"));
                }
                return emails;
            }
        }

        public List<String> GetUserEmailsByIds(Db db, List<int> userIds)
        {
            if (userIds.Count == 0)
            {
                return [];
            }
            else
            {
                string sql = @"SELECT EmailAddress
                           FROM [User] WHERE UserID IN ({0})";

                using SqlCommand command = new(sql, db.Connection);

                var idParameterList = new List<string>(userIds.Count);
                for (int index = 0; index < userIds.Count; index++)
                {
                    idParameterList[index] = $"@UserId{index}";
                    command.Parameters.Add(idParameterList[index], SqlDbType.Int).Value = userIds[index];
                }
                command.CommandText = string.Format(sql, string.Join(",", idParameterList));
                using SqlDataReader reader = db.ExecuteReader(command);

                List<string> emails = [];
                while (reader.Read())
                {
                    emails.Add(reader.GetString("EmailAddress"));
                }
                return emails;
            }
        }
    }
}
