using BusinessOperationsSaaS.Application.Subscriptions.DTOs;
using BusinessOperationsSaaS.Application.Subscriptions.Interfaces;
using BusinessOperationsSaaS.Domain.Entities;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperationsSaaS.Infrastructure.Subscriptions;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;

    public SubscriptionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubscriptionPlanDto>> GetPlansAsync()
    {
        return await _context.SubscriptionPlans
            .Where(x => x.IsActive)
            .OrderBy(x => x.MonthlyPrice)
            .Select(x => new SubscriptionPlanDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                MonthlyPrice = x.MonthlyPrice,
                YearlyPrice = x.YearlyPrice,
                MaxEmployees = x.MaxEmployees,
                MaxProducts = x.MaxProducts,
                MaxTasks = x.MaxTasks,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<SubscriptionPlanDto?> GetPlanByIdAsync(Guid id)
    {
        return await _context.SubscriptionPlans
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new SubscriptionPlanDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                MonthlyPrice = x.MonthlyPrice,
                YearlyPrice = x.YearlyPrice,
                MaxEmployees = x.MaxEmployees,
                MaxProducts = x.MaxProducts,
                MaxTasks = x.MaxTasks,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SubscriptionDto?> GetCurrentSubscriptionAsync(Guid companyId)
    {
        return await _context.Subscriptions
            .Include(x => x.SubscriptionPlan)
            .Where(x =>
                x.CompanyId == companyId &&
                x.Status == "Active")
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SubscriptionDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                SubscriptionPlanId = x.SubscriptionPlanId,
                Status = x.Status,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TrialEndsAt = x.TrialEndsAt,
                StripeCustomerId = x.StripeCustomerId,
                StripeSubscriptionId = x.StripeSubscriptionId,
                CreatedAt = x.CreatedAt,
                SubscriptionPlan = new SubscriptionPlanDto
                {
                    Id = x.SubscriptionPlan.Id,
                    Name = x.SubscriptionPlan.Name,
                    Description = x.SubscriptionPlan.Description,
                    MonthlyPrice = x.SubscriptionPlan.MonthlyPrice,
                    YearlyPrice = x.SubscriptionPlan.YearlyPrice,
                    MaxEmployees = x.SubscriptionPlan.MaxEmployees,
                    MaxProducts = x.SubscriptionPlan.MaxProducts,
                    MaxTasks = x.SubscriptionPlan.MaxTasks,
                    IsActive = x.SubscriptionPlan.IsActive
                }
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SubscriptionDto?> CreateSubscriptionAsync(
        Guid companyId,
        CreateSubscriptionDto dto)
    {
        var plan = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(x =>
                x.Id == dto.SubscriptionPlanId &&
                x.IsActive);

        if (plan is null)
        {
            return null;
        }

        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(x =>
                x.CompanyId == companyId &&
                x.Status == "Active");

        if (existingSubscription is not null)
        {
            return null;
        }

        var now = DateTime.UtcNow;

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            SubscriptionPlanId = plan.Id,
            Status = "Active",
            StartDate = now,
            TrialEndsAt = dto.TrialDays > 0
                ? now.AddDays(dto.TrialDays)
                : null,
            CreatedAt = now
        };

        _context.Subscriptions.Add(subscription);

        await _context.SaveChangesAsync();

        return await GetSubscriptionDtoAsync(subscription.Id);
    }

    public async Task<SubscriptionDto?> UpdateSubscriptionAsync(
        Guid companyId,
        Guid subscriptionId,
        UpdateSubscriptionDto dto)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(x =>
                x.Id == subscriptionId &&
                x.CompanyId == companyId);

        if (subscription is null)
        {
            return null;
        }

        var plan = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(x =>
                x.Id == dto.SubscriptionPlanId &&
                x.IsActive);

        if (plan is null)
        {
            return null;
        }

        subscription.SubscriptionPlanId = plan.Id;
        subscription.Status = dto.Status;

        await _context.SaveChangesAsync();

        return await GetSubscriptionDtoAsync(subscription.Id);
    }

    private async Task<SubscriptionDto?> GetSubscriptionDtoAsync(Guid subscriptionId)
    {
        return await _context.Subscriptions
            .Include(x => x.SubscriptionPlan)
            .Where(x => x.Id == subscriptionId)
            .Select(x => new SubscriptionDto
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                SubscriptionPlanId = x.SubscriptionPlanId,
                Status = x.Status,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TrialEndsAt = x.TrialEndsAt,
                StripeCustomerId = x.StripeCustomerId,
                StripeSubscriptionId = x.StripeSubscriptionId,
                CreatedAt = x.CreatedAt,
                SubscriptionPlan = new SubscriptionPlanDto
                {
                    Id = x.SubscriptionPlan.Id,
                    Name = x.SubscriptionPlan.Name,
                    Description = x.SubscriptionPlan.Description,
                    MonthlyPrice = x.SubscriptionPlan.MonthlyPrice,
                    YearlyPrice = x.SubscriptionPlan.YearlyPrice,
                    MaxEmployees = x.SubscriptionPlan.MaxEmployees,
                    MaxProducts = x.SubscriptionPlan.MaxProducts,
                    MaxTasks = x.SubscriptionPlan.MaxTasks,
                    IsActive = x.SubscriptionPlan.IsActive
                }
            })
            .FirstOrDefaultAsync();
    }
}

