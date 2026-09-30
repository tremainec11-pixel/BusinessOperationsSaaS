using BusinessOperationsSaaS.Application.Subscriptions.DTOs;
using BusinessOperationsSaaS.Application.Subscriptions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IStripeService _stripeService;

    public SubscriptionsController(
        ISubscriptionService subscriptionService,
        IStripeService stripeService)
    {
        _subscriptionService = subscriptionService;
        _stripeService = stripeService;
    }

    [HttpGet("plans")]
    public async Task<IActionResult> GetPlans()
    {
        var plans = await _subscriptionService.GetPlansAsync();

        return Ok(plans);
    }

    [HttpGet("plans/{id:guid}")]
    public async Task<IActionResult> GetPlanById(Guid id)
    {
        var plan = await _subscriptionService.GetPlanByIdAsync(id);

        if (plan is null)
        {
            return NotFound(new
            {
                message = "Subscription plan not found."
            });
        }

        return Ok(plan);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentSubscription()
    {
        var companyId = PublicCompanyId;

        

        var subscription =
            await _subscriptionService.GetCurrentSubscriptionAsync(
                companyId);

        if (subscription is null)
        {
            return NotFound(new
            {
                message = "No active subscription found for this company."
            });
        }

        return Ok(subscription);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubscription(
        [FromBody] CreateSubscriptionDto dto)
    {
        var companyId = PublicCompanyId;

        

        var subscription =
            await _subscriptionService.CreateSubscriptionAsync(
                companyId,
                dto);

        if (subscription is null)
        {
            return BadRequest(new
            {
                message =
                    "The subscription could not be created. " +
                    "The plan may not exist or the company may already have an active subscription."
            });
        }

        return CreatedAtAction(
            nameof(GetCurrentSubscription),
            null,
            subscription);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckout(
        [FromBody] CreateSubscriptionDto dto)
    {
        var companyId = PublicCompanyId;

        

        try
        {
            var checkoutUrl =
                await _stripeService.CreateCheckoutSessionAsync(
                    companyId,
                    dto.SubscriptionPlanId);

            return Ok(new
            {
                checkoutUrl
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSubscription(
        Guid id,
        [FromBody] UpdateSubscriptionDto dto)
    {
        var companyId = PublicCompanyId;

        

        var subscription =
            await _subscriptionService.UpdateSubscriptionAsync(
                companyId,
                id,
                dto);

        if (subscription is null)
        {
            return NotFound(new
            {
                message = "Subscription or subscription plan not found."
            });
        }

        return Ok(subscription);
    }

    private static readonly Guid PublicCompanyId = Guid.Parse("feb88165-b70c-429e-83a3-94d412dca312");
}

