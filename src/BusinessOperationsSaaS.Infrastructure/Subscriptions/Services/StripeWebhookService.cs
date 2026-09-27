using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace BusinessOperationsSaaS.Infrastructure.Subscriptions.Services;

public class StripeWebhookService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public StripeWebhookService(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task HandleAsync(
        string json,
        string stripeSignature)
    {
        var webhookSecret =
            _configuration["StripeSettings:WebhookSecret"];

        if (string.IsNullOrWhiteSpace(webhookSecret))
        {
            throw new InvalidOperationException(
                "Stripe WebhookSecret is not configured.");
        }

        var stripeEvent = EventUtility.ConstructEvent(
            json,
            stripeSignature,
            webhookSecret
        );

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
                await HandleCheckoutSessionCompletedAsync(
                    stripeEvent);
                break;

            case EventTypes.CustomerSubscriptionUpdated:
                await HandleSubscriptionUpdatedAsync(
                    stripeEvent);
                break;

            case EventTypes.CustomerSubscriptionDeleted:
                await HandleSubscriptionDeletedAsync(
                    stripeEvent);
                break;
        }
    }

    private async Task HandleCheckoutSessionCompletedAsync(
        Event stripeEvent)
    {
        var session =
            stripeEvent.Data.Object as Session;

        if (session is null)
        {
            return;
        }

        if (session.Metadata is null ||
            !session.Metadata.TryGetValue(
                "companyId",
                out var companyIdValue) ||
            !Guid.TryParse(
                companyIdValue,
                out var companyId))
        {
            return;
        }

        if (session.Metadata is null ||
            !session.Metadata.TryGetValue(
                "subscriptionPlanId",
                out var planIdValue) ||
            !Guid.TryParse(
                planIdValue,
                out var subscriptionPlanId))
        {
            return;
        }

        var subscription =
            await _context.Subscriptions
                .FirstOrDefaultAsync(x =>
                    x.CompanyId == companyId &&
                    x.Status == "Active");

        if (subscription is null)
        {
            subscription = new Domain.Entities.Subscription
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                SubscriptionPlanId = subscriptionPlanId,
                Status = "Active",
                StartDate = DateTime.UtcNow,
                StripeCustomerId =
                    session.CustomerId,
                StripeSubscriptionId =
                    session.SubscriptionId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscription);
        }
        else
        {
            subscription.SubscriptionPlanId =
                subscriptionPlanId;

            subscription.StripeCustomerId =
                session.CustomerId;

            subscription.StripeSubscriptionId =
                session.SubscriptionId;

            subscription.Status = "Active";
        }

        await _context.SaveChangesAsync();
    }

    private async Task HandleSubscriptionUpdatedAsync(
        Event stripeEvent)
    {
        var stripeSubscription =
            stripeEvent.Data.Object as Stripe.Subscription;

        if (stripeSubscription is null)
        {
            return;
        }

        var subscription =
            await _context.Subscriptions
                .FirstOrDefaultAsync(x =>
                    x.StripeSubscriptionId ==
                    stripeSubscription.Id);

        if (subscription is null)
        {
            return;
        }

        subscription.Status =
            stripeSubscription.Status;

        subscription.EndDate =
    stripeSubscription.CancelAt;

        await _context.SaveChangesAsync();
    }

    private async Task HandleSubscriptionDeletedAsync(
        Event stripeEvent)
    {
        var stripeSubscription =
            stripeEvent.Data.Object as Stripe.Subscription;

        if (stripeSubscription is null)
        {
            return;
        }

        var subscription =
            await _context.Subscriptions
                .FirstOrDefaultAsync(x =>
                    x.StripeSubscriptionId ==
                    stripeSubscription.Id);

        if (subscription is null)
        {
            return;
        }

        subscription.Status = "Canceled";
        subscription.EndDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}