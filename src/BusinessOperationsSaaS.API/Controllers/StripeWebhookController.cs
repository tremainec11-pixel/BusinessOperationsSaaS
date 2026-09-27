using BusinessOperationsSaaS.Infrastructure.Subscriptions.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebhookController : ControllerBase
{
    private readonly StripeWebhookService _webhookService;

    public StripeWebhookController(
        StripeWebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        using var reader =
            new StreamReader(Request.Body);

        var json = await reader.ReadToEndAsync();

        var signature =
            Request.Headers["Stripe-Signature"]
                .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(signature))
        {
            return BadRequest(new
            {
                message =
                    "Stripe-Signature header is missing."
            });
        }

        try
        {
            await _webhookService.HandleAsync(
                json,
                signature);

            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest(new
            {
                message =
                    "Invalid Stripe webhook signature."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}
