using BusinessOperationsSaaS.Application.Subscriptions.Interfaces;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

namespace BusinessOperationsSaaS.Infrastructure.Subscriptions.Services;

public class StripeService : IStripeService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public StripeService(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> CreateCheckoutSessionAsync(
        Guid companyId,
        Guid subscriptionPlanId)
    {
        var plan = await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == subscriptionPlanId &&
                x.IsActive);

        if (plan is null)
        {
            throw new InvalidOperationException(
                "Subscription plan not found.");
        }

        if (plan.MonthlyPrice <= 0)
        {
            throw new InvalidOperationException(
                "This subscription plan does not require payment.");
        }

        var secretKey =
            _configuration["StripeSettings:SecretKey"];

        Console.WriteLine(
            $"StripeService SecretKey loaded: {!string.IsNullOrWhiteSpace(secretKey)}"
        );

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException(
                "Stripe SecretKey is not configured.");
        }

        Stripe.StripeConfiguration.ApiKey = secretKey;

        var options = new SessionCreateOptions
        {
            Mode = "subscription",

            SuccessUrl =
                "http://localhost:5173/subscription/success?session_id={CHECKOUT_SESSION_ID}",

            CancelUrl =
                "http://localhost:5173/subscription/cancel",

            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,

                    PriceData =
                        new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",

                            UnitAmountDecimal =
                                plan.MonthlyPrice * 100,

                            Recurring =
                                new SessionLineItemPriceDataRecurringOptions
                                {
                                    Interval = "month"
                                },

                            ProductData =
                                new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = plan.Name,
                                    Description = plan.Description
                                }
                        }
                }
            ],

            Metadata = new Dictionary<string, string>
            {
                ["companyId"] = companyId.ToString(),
                ["subscriptionPlanId"] =
                    subscriptionPlanId.ToString()
            }
        };

        var service = new SessionService();

        var session = await service.CreateAsync(options);

        return session.Url;
    }
}