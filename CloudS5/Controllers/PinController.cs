using Microsoft.AspNetCore.Mvc;

public class PinController : Controller
{
    private readonly PinService _pinService;

    public PinController(PinService pinService)
    {
        _pinService = pinService;
    }

    // Action pour générer un PIN et le stocker dans la session
    [HttpGet("generate-pin")]
    public IActionResult GeneratePin()
    {
        int pin = _pinService.GeneratePin();  // Génère le code PIN
        _pinService.StorePinInSession(pin, 5); // Stocke le PIN dans la session avec une validité de 5 minutes

        return Ok(new { Pin = pin });
    }

    // Action pour valider un PIN
    [HttpPost("validate-pin")]
    public IActionResult ValidatePin([FromBody] int pin)
    {
        if (!_pinService.IsPinValid())
        {
            return BadRequest("The PIN has expired.");
        }

        bool isValid = _pinService.ValidatePin(pin);

        if (isValid)
        {
            return Ok("PIN is valid.");
        }
        else
        {
            return BadRequest("Invalid PIN.");
        }
    }
}
