// using Microsoft.AspNetCore.Mvc;

// [Route("api/[controller]")]
// [ApiController]
// public class EmailController : ControllerBase
// {
//     private readonly EmailService _emailService;
//     private readonly PinService _pinService;

//     public EmailController(EmailService emailService, PinService pinService)
//     {
//         _emailService = emailService;
//         _pinService = pinService;
//     }

//     // Action pour générer un code PIN et l'envoyer par email
//     [HttpPost("send-pin-email")]
//     public IActionResult SendPinEmail([FromBody] SendPinEmailRequest request)
//     {
//         try
//         {
//             // Génère le code PIN
//             int pin = _pinService.GeneratePin();

//             // Enregistre le PIN dans la session avec une durée de vie de 10 minutes (par exemple)
//             _pinService.StorePinInSession(pin, 90);

//             // Prépare le contenu HTML de l'email
//             string htmlContent = $"<h1>Your PIN Code</h1><p>Your PIN code is: <strong>{pin}</strong></p>";

//             // Envoie l'email avec le code PIN
//             _emailService.SendEmail(request.RecipientEmail, "Your PIN Code", htmlContent);

//             return Ok("PIN sent to the email successfully.");
//         }
//         catch (Exception ex)
//         {
//             return StatusCode(500, $"Error sending PIN: {ex.Message}");
//         }
//     }
// }

// // Modèle pour recevoir les données dans le corps de la requête
// public class SendPinEmailRequest
// {
//     public string RecipientEmail { get; set; }
// }


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
    [HttpPost("store-user")]
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
    [HttpPost("validate-pin")]
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
