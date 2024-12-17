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

    // Action pour générer un code PIN et l'envoyer par email
    [HttpPost("send-pin-email")]
    public IActionResult SendPinEmail([FromBody] SendPinEmailRequest request)
    {
        try
        {
            // Génère le code PIN
            int pin = _pinService.GeneratePin();

            // Enregistre le PIN dans la session avec une durée de vie de 10 minutes (par exemple)
            _pinService.StorePinInSession(pin, 90);

            // Prépare le contenu HTML de l'email
            string htmlContent = $"<h1>Your PIN Code</h1><p>Your PIN code is: <strong>{pin}</strong></p>";

            // Envoie l'email avec le code PIN
            _emailService.SendEmail(request.RecipientEmail, "Your PIN Code", htmlContent);

            return Ok("PIN sent to the email successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending PIN: {ex.Message}");
        }
    }
}

// Modèle pour recevoir les données dans le corps de la requête
public class SendPinEmailRequest
{
    public string RecipientEmail { get; set; }
}
