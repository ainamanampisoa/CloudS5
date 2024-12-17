using System;
using Npgsql;

namespace Kidoro.Models
{
    public class DatabaseConnection
    {
        private string postgresConnectionString = "Host=localhost;Database=clouds5;Username=postgres;Password=aina;Pooling=true;";

        public NpgsqlConnection GetPostgresConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(postgresConnectionString);
            return connection;
        }

    }
}
