using Microsoft.Data.SqlClient;

namespace ReleaseMonkey.src.Repositories
{
    public class Db
    {
        private readonly SqlConnection conn;

        public SqlConnection Connection { get => conn; }

        public Db()
        {
            SqlConnectionStringBuilder builder = new()
            {
                // TODO: Read from Environment variable
                DataSource = "localhost\\SQLEXPRESS",
                UserID = "user",
                Password = "password",
                InitialCatalog = "release_monkey_db",
                TrustServerCertificate = true
            };

            conn = new SqlConnection(builder.ConnectionString);
            conn.Open();
        }

        public SqlDataReader ExecuteReader(SqlCommand cmd)
        {
            return cmd.ExecuteReader();
        }

        ~Db()
        {
            conn.Close();
        }
        
    }
}
