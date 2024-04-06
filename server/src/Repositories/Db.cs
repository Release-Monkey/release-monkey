using System;
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
                DataSource = Environment.GetEnvironmentVariable("DB_HOST"),
                UserID = Environment.GetEnvironmentVariable("DB_USER"),
                Password = Environment.GetEnvironmentVariable("DB_PASS"),
                InitialCatalog = Environment.GetEnvironmentVariable("DB_NAME"),
                TrustServerCertificate = bool.Parse(Environment.GetEnvironmentVariable("DB_TRUST_CERT"))
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
