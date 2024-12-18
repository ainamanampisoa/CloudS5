using System;
using Npgsql;

namespace CloudS5.Models
{
    public class DatabaseConnection
    {
        private string postgresConnectionString = "Host=postgres-db;Port=5432;Database=clouds5;Username=postgres;Password=postgres";

        public NpgsqlConnection GetPostgresConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(postgresConnectionString);
            return connection;
        }

    }
}
