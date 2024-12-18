using System;
using Npgsql;

public class DatabaseConnection
{
    public NpgsqlConnection GetPostgresConnection()
    {
        // Récupérer les informations de connexion à partir des variables d'environnement Docker
        string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"; // "postgres-db" dans Docker
        string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "clouds5";
        string user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        string password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";

        // Créer la chaîne de connexion PostgreSQL à partir des variables d'environnement
        string postgresConnectionString = $"Host={host};Port={port};Database={dbName};Username={user};Password={password};Pooling=true;";

        // Créer et retourner la connexion
        NpgsqlConnection connection = new NpgsqlConnection(postgresConnectionString);
        return connection;
    }
}
