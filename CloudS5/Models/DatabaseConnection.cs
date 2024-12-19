using System;
using Npgsql;

public class DatabaseConnection
{
    private string postgresConnectionString = "Host=localhost;Database=clouds5;Username=postgres;Password=aina;Pooling=true;";

    public NpgsqlConnection GetPostgresConnection()
    {
        NpgsqlConnection connection = new NpgsqlConnection(postgresConnectionString);
        return connection;
    }
}