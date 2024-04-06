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

        public List<String> GetUserEmailsByIds(SqlTransaction transaction, Db db, List<int> userIds)
        {
            string sql = @"SELECT EmailAddress
                           FROM [User] WHERE UserID IN ({0})";

            using SqlCommand command = new(sql, db.Connection, transaction);

            var idParameterList = new List<string>();
            var index = 0;
            foreach (var userId in userIds)
            {
                var paramName = "@idParam" + index;
                command.Parameters.AddWithValue(paramName, userId);
                idParameterList.Add(paramName);
                index++;
            }
            command.CommandText = String.Format(sql, string.Join(",", idParameterList));
            using SqlDataReader reader = db.ExecuteReader(command);

            List<String> emails = new List<String>();
            while (reader.Read())
            {
                 emails.Add(reader.GetString("EmailAddress"));
            }
            return emails;
        }
    }
}
