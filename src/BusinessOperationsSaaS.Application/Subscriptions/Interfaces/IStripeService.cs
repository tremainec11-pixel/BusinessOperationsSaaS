namespace BusinessOperationsSaaS.Application.Subscriptions.Interfaces;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(
        Guid companyId,
        Guid subscriptionPlanId);
}