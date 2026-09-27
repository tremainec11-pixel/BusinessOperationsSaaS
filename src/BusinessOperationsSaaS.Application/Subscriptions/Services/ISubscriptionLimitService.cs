namespace BusinessOperationsSaaS.Application.Subscriptions.Services;

public interface ISubscriptionLimitService
{
    Task<bool> CanCreateEmployeeAsync(Guid companyId);
    Task<bool> CanCreateProductAsync(Guid companyId);
    Task<bool> CanCreateTaskAsync(Guid companyId);
}