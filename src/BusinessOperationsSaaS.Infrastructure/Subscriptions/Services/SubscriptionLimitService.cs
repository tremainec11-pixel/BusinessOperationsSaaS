using BusinessOperationsSaaS.Application.Subscriptions.Services;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperationsSaaS.Infrastructure.Subscriptions.Services;

public class SubscriptionLimitService : ISubscriptionLimitService
{
    private readonly AppDbContext _context;

    public SubscriptionLimitService(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> CanCreateEmployeeAsync(Guid companyId)
    {
        return CanCreateAsync(
            companyId,
            plan => plan.MaxEmployees,
            subscription =>
                _context.Employees.CountAsync(x =>
                    x.CompanyId == subscription.CompanyId &&
                    x.IsActive));
    }

    public Task<bool> CanCreateProductAsync(Guid companyId)
    {
        return CanCreateAsync(
            companyId,
            plan => plan.MaxProducts,
            subscription =>
                _context.Products.CountAsync(x =>
                    x.CompanyId == subscription.CompanyId &&
                    x.IsActive));
    }

    public Task<bool> CanCreateTaskAsync(Guid companyId)
    {
        return CanCreateAsync(
            companyId,
            plan => plan.MaxTasks,
            subscription =>
                _context.Tasks.CountAsync(x =>
                    x.CompanyId == subscription.CompanyId &&
                    !x.IsCompleted));
    }

    private async Task<bool> CanCreateAsync(
        Guid companyId,
        Func<BusinessOperationsSaaS.Domain.Entities.SubscriptionPlan, int> limitSelector,
        Func<BusinessOperationsSaaS.Domain.Entities.Subscription, Task<int>> countSelector)
    {
        var subscription = await _context.Subscriptions
            .Include(x => x.SubscriptionPlan)
            .Where(x =>
                x.CompanyId == companyId &&
                x.Status == "Active")
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (subscription is null)
        {
            return false;
        }

        var limit = limitSelector(subscription.SubscriptionPlan);

        if (limit == -1)
        {
            return true;
        }

        var currentCount = await countSelector(subscription);

        return currentCount < limit;
    }
}
