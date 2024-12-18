using Microsoft.AspNetCore.Http;
using System;

public class PinService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PinService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Génère un code PIN aléatoire de 4 chiffres
    public int GeneratePin()
    {
        Random random = new Random();
        int pin = random.Next(1000, 10000); // Génère un nombre entre 1000 et 9999
        return pin;
    }

    // Stocke le code PIN dans la session avec une durée de vie configurée
    public void StorePinInSession(int pin, int sessionDurationInSeconds)
    {
        var context = _httpContextAccessor.HttpContext;

        // Enregistrer le PIN dans la session
        context.Session.SetInt32("PinCode", pin);

        // Enregistrer l'heure d'expiration de la session
        var expirationTime = DateTime.Now.AddSeconds(sessionDurationInSeconds);
        context.Session.SetString("PinExpirationTime", expirationTime.ToString("o"));
    }

    // Vérifie si le code PIN est encore valide
    public bool IsPinValid()
    {
        var context = _httpContextAccessor.HttpContext;
        var expirationTimeString = context.Session.GetString("PinExpirationTime");

        if (DateTime.TryParse(expirationTimeString, out DateTime expirationTime))
        {
            return expirationTime > DateTime.Now; // Si l'heure actuelle est avant l'heure d'expiration
        }

        return false; // Si la session a expiré
    }

    // Vérifie si le PIN fourni est valide
    public bool ValidatePin(int pin)
    {
        var context = _httpContextAccessor.HttpContext;
        int storedPin = context.Session.GetInt32("PinCode") ?? 0;

        return storedPin == pin;
    }
}
