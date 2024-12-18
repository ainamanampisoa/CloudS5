using System;
using System.Security.Cryptography;
using System.Text;

public class Hashing
{
    // Fonction pour hasher une chaîne
    public static string HashString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            byte[] hashBytes = sha256.ComputeHash(bytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }


    // Fonction pour comparer deux hashs
    public static bool CompareHashes(string hash1, string hash2)
    {
        return string.Equals(hash1, hash2, StringComparison.Ordinal);
    }
}