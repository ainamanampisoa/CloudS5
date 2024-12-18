using Npgsql; // Bibliothèque PostgreSQL
using System;

public class Utilisateur
{
    // Propriétés du modèle
    public int IdUser { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int IdType { get; set; }

    // Méthode pour insérer un utilisateur dans la base de données
    public void InsertUtilisateur()
    {
        try
        {
            // Ouvrir une connexion avec PostgreSQL
            using (var connection = new DatabaseConnection().GetPostgresConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // Requête SQL pour insérer un utilisateur
                    command.CommandText = @"
                        INSERT INTO utilisateur (email, username, password, id_type) 
                        VALUES (@Email, @Username, @Password, @IdType)";
                    
                    // Ajouter les paramètres à la commande
                    command.Parameters.AddWithValue("Email", Email);
                    command.Parameters.AddWithValue("Username", Username);
                    command.Parameters.AddWithValue("Password", Password);
                    command.Parameters.AddWithValue("IdType", IdType);

                    // Exécuter la commande
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            // Gérer les exceptions
            throw new Exception($"Erreur lors de l'insertion de l'utilisateur : {ex.Message}", ex);
        }
    }
}