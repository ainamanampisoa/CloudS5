using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly PinService _pinService;

    public EmailController(EmailService emailService, PinService pinService)
    {
        _emailService = emailService;
        _pinService = pinService;
    }

    // Action pour enregistrer l'utilisateur et envoyer automatiquement un PIN
    [HttpPost("sendPin")]
    public IActionResult StoreUser([FromBody] StoreUserRequest request)
    {
        try
        {
            // Générer un code PIN
            int pin = _pinService.GeneratePin();

            // Stocker les informations utilisateur et le PIN dans la session
            var context = HttpContext;
            context.Session.SetString("UserEmail", request.Email);
            context.Session.SetString("UserPassword", request.Password);
            context.Session.SetString("Username", request.Username);
            context.Session.SetInt32("UserType", request.IdType);
            _pinService.StorePinInSession(pin, 300); // Code PIN valide pendant 5 minutes

            // Envoyer un e-mail avec le code PIN
            string htmlContent = $"<h1>Your PIN Code</h1><p>Your PIN code is: <strong>{pin}</strong></p>";
            _emailService.SendEmail(request.Email, "Your PIN Code", htmlContent);

            return Ok(new { message = "User data stored and PIN sent to the email successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error storing user or sending PIN: {ex.Message}");
        }
    }

    // Action pour valider le code PIN
    [HttpPost("pinInscription")]
    public IActionResult ValidatePin([FromBody] ValidatePinRequest request)
    {
        try
        {
            var context = HttpContext;

            // Vérifier si le PIN est valide et correspond à celui de la session
            if (!_pinService.IsPinValid())
            {
                // Si le PIN a expiré
                context.Session.Clear(); // Supprimer les données de session
                return BadRequest(new { message = "The PIN has expired. Please request a new one." });
            }

            if (!_pinService.ValidatePin(request.PinCode))
            {
                // Si le PIN est incorrect
                context.Session.Clear(); // Supprimer les données de session
                return BadRequest(new { message = "Invalid PIN. Please try again." });
            }

            // Récupérer les informations de l'utilisateur depuis la session
            string email = context.Session.GetString("UserEmail");
            string password = context.Session.GetString("UserPassword");
            string username = context.Session.GetString("Username");
            int? idType = context.Session.GetInt32("UserType");

            // Vérifier si toutes les données nécessaires sont disponibles
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username) || idType == null)
            {
                context.Session.Clear(); // Supprimer les données de session
                return BadRequest(new { message = "User session data is incomplete." });
            }

            // Créer une instance de la classe Utilisateur
            var utilisateur = new Utilisateur
            {
                Email = email,
                Password = password,
                Username = username,
                IdType = idType.Value
            };

            // Insérer l'utilisateur dans la base de données
            utilisateur.InsertUtilisateur();

            // Effacer la session après validation
            context.Session.Clear();

            return Ok(new { message = "User successfully registered." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error validating PIN or registering user: {ex.Message}");
        }
    }

    // Action pour vérifier l'utilisateur et envoyer un PIN
    [HttpPost("checkAndSendPin")]
    public IActionResult CheckAndSendPin([FromBody] CheckUserRequest request)
    {
        try
        {
            var context = HttpContext;

            // Initialiser ou récupérer le compteur de tentatives
            int remainingAttempts = context.Session.GetInt32("RemainingAttempts") ?? 3;

            if (remainingAttempts <= 0)
            {
                return BadRequest(new { message = "Too many failed attempts. Please try again later." });
            }
            
            // Vérifier si l'utilisateur existe
            bool isValidUser = Utilisateur.CheckUtilisateur(request.Email, request.Password);

            if (!isValidUser)
            {
                // Décrémenter le compteur si le code PIN est incorrect
                remainingAttempts--;
                context.Session.SetInt32("RemainingAttempts", remainingAttempts);

                return Unauthorized(new { message = $"Invalid PIN. Remaining attempts: {remainingAttempts}" });
            }

            // Générer un code PIN
            int pin = _pinService.GeneratePin();

            // Stocker le code PIN dans la session
            context.Session.SetString("UserEmail", request.Email);
            _pinService.StorePinInSession(pin, 300); // Code PIN valide pendant 5 minutes

            // Envoyer un e-mail avec le code PIN
            string htmlContent = $"<h1>Your PIN Code</h1><p>Your PIN code is: <strong>{pin}</strong></p>";
            _emailService.SendEmail(request.Email, "Your PIN Code", htmlContent);

            return Ok(new { message = "PIN sent to the email successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error checking user or sending PIN: {ex.Message}");
        }
    }

    [HttpPost("pinConnection")]
    public IActionResult ValidatePinC([FromBody] ValidatePinRequest request)
    {
        try
        {
            var context = HttpContext;

            // Initialiser ou récupérer le compteur de tentatives
            int remainingAttempts = context.Session.GetInt32("RemainingAttempts") ?? 3;

            if (remainingAttempts <= 0)
            {
                return BadRequest(new { message = "Too many failed attempts. Please try again later." });
            }

            // Vérifier si le PIN est valide
            if (!_pinService.IsPinValid())
            {
                context.Session.Clear(); // Supprimer les données de session
                return BadRequest(new { message = "The PIN has expired. Please request a new one." });
            }

            if (!_pinService.ValidatePin(request.PinCode))
            {
                // Décrémenter le compteur si le code PIN est incorrect
                remainingAttempts--;
                context.Session.SetInt32("RemainingAttempts", remainingAttempts);

                return BadRequest(new { message = $"Invalid PIN. Remaining attempts: {remainingAttempts}" });
            }

            // Réinitialiser les tentatives pour cet utilisateur après un succès
            context.Session.SetInt32("RemainingAttempts", 3);

            return Ok(new { message = "Connexion reussie." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error validating PIN or registering user: {ex.Message}");
        }
    }
}

// Modèle pour la requête d'enregistrement d'utilisateur
public class StoreUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
    public int IdType { get; set; }
}

// Modèle pour la validation du code PIN
public class ValidatePinRequest
{
    public int PinCode { get; set; }
}

// Modèle pour la vérification de l'utilisateur
public class CheckUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}