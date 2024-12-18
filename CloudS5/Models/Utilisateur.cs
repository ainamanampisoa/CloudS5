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
            // Hasher le mot de passe avant de l'insérer dans la base de données
            string hashedPassword = Hashing.HashString(Password);

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
                    command.Parameters.AddWithValue("Password", hashedPassword); // Utiliser le mot de passe hashé
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

    // Méthode pour vérifier si un utilisateur existe avec l'email et le mot de passe fournis
    public static bool CheckUtilisateur(string email, string password)
    {
        try
        {
            // Ouvrir une connexion avec PostgreSQL
            using (var connection = new DatabaseConnection().GetPostgresConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // Requête SQL pour récupérer le mot de passe hashé de l'utilisateur
                    command.CommandText = @"
                        SELECT password 
                        FROM utilisateur 
                        WHERE email = @Email";
                    
                    // Ajouter les paramètres à la commande
                    command.Parameters.AddWithValue("Email", email);

                    // Exécuter la commande et récupérer le mot de passe hashé
                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        return false; // Utilisateur non trouvé
                    }

                    string storedHashedPassword = result.ToString();

                    // Comparer les hashs
                    return Hashing.CompareHashes(storedHashedPassword, Hashing.HashString(password));
                }
            }
        }
        catch (Exception ex)
        {
            // Gérer les exceptions
            throw new Exception($"Erreur lors de la vérification de l'utilisateur : {ex.Message}", ex);
        }
    }

    // Méthode pour mettre à jour les informations utilisateur dans la base de données
    public bool UpdateUtilisateur()
    {
        try
        {
            // Hasher le mot de passe avant de le mettre à jour
            string hashedPassword = Hashing.HashString(Password);

            using (var connection = new DatabaseConnection().GetPostgresConnection()) // Assurez-vous que DatabaseConnection est correctement implémentée
            {
                connection.Open(); // Ouvrir la connexion une seule fois

                using (var command = connection.CreateCommand())
                {
                    string query = "UPDATE utilisateur SET password = @Password, username = @Username, id_type = @IdType WHERE email = @Email";
                    command.CommandText = query;

                    // Ajout des paramètres
                    command.Parameters.AddWithValue("@Password", hashedPassword); // Utiliser le mot de passe hashé
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@IdType", IdType);
                    command.Parameters.AddWithValue("@Email", Email);

                    // Exécuter la commande
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Retourne true si au moins une ligne a été affectée
                }
            }
        }
        catch (Exception ex)
        {
            // Gérer les exceptions
            throw new Exception($"Erreur lors de la mise à jour de l'utilisateur : {ex.Message}", ex);
        }
    }
}