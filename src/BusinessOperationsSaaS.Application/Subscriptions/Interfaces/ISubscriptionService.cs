using BusinessOperationsSaaS.Application.Subscriptions.DTOs;

namespace BusinessOperationsSaaS.Application.Subscriptions.Interfaces;

public interface ISubscriptionService
{
    Task<List<SubscriptionPlanDto>> GetPlansAsync();

    Task<SubscriptionPlanDto?> GetPlanByIdAsync(Guid id);

    Task<SubscriptionDto?> GetCurrentSubscriptionAsync(Guid companyId);

    Task<SubscriptionDto?> CreateSubscriptionAsync(
        Guid companyId,
        CreateSubscriptionDto dto);

    Task<SubscriptionDto?> UpdateSubscriptionAsync(
        Guid companyId,
        Guid subscriptionId,
        UpdateSubscriptionDto dto);
}
