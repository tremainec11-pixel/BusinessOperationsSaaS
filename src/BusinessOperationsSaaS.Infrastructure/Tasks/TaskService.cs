using BusinessOperationsSaaS.Application.Tasks.DTOs;
using BusinessOperationsSaaS.Application.Tasks.Interfaces;
using BusinessOperationsSaaS.Application.Subscriptions.Services;
using BusinessOperationsSaaS.Domain.Entities;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperationsSaaS.Infrastructure.Tasks;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly ISubscriptionLimitService _subscriptionLimitService;

    public TaskService(
        AppDbContext context,
        ISubscriptionLimitService subscriptionLimitService)
    {
        _context = context;
        _subscriptionLimitService = subscriptionLimitService;
    }

    public async Task<List<TaskResponse>> GetAllAsync(Guid companyId)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.IsCompleted)
            .ThenBy(x => x.DueDate)
            .ThenBy(x => x.CreatedAt)
            .Select(x => new TaskResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                AssignedToEmployeeId = x.AssignedToEmployeeId,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status,
                Priority = x.Priority,
                DueDate = x.DueDate,
                IsCompleted = x.IsCompleted,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<TaskResponse?> GetByIdAsync(
        Guid companyId,
        Guid taskId)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(x =>
                x.Id == taskId &&
                x.CompanyId == companyId)
            .Select(x => new TaskResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                AssignedToEmployeeId = x.AssignedToEmployeeId,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status,
                Priority = x.Priority,
                DueDate = x.DueDate,
                IsCompleted = x.IsCompleted,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TaskResponse> CreateAsync(
        Guid companyId,
        CreateTaskRequest request)
    {
        var canCreate =
            await _subscriptionLimitService
                .CanCreateTaskAsync(companyId);

        if (!canCreate)
        {
            throw new InvalidOperationException(
                "Task limit reached for the current subscription plan.");
        }

        if (request.AssignedToEmployeeId.HasValue)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(x =>
                    x.Id == request.AssignedToEmployeeId.Value &&
                    x.CompanyId == companyId);

            if (!employeeExists)
            {
                throw new InvalidOperationException(
                    "Assigned employee does not belong to this company.");
            }
        }

        var task = new Domain.Entities.Task
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            AssignedToEmployeeId = request.AssignedToEmployeeId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status.Trim(),
            Priority = request.Priority.Trim(),
            DueDate = request.DueDate.HasValue
            ? DateTime.SpecifyKind(request.DueDate.Value, DateTimeKind.Utc)
            : null,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);

        await _context.SaveChangesAsync();

        return new TaskResponse
        {
            Id = task.Id,
            CompanyId = task.CompanyId,
            AssignedToEmployeeId = task.AssignedToEmployeeId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<TaskResponse?> UpdateAsync(
        Guid companyId,
        Guid taskId,
        UpdateTaskRequest request)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(x =>
                x.Id == taskId &&
                x.CompanyId == companyId);

        if (task is null)
        {
            return null;
        }

        if (request.AssignedToEmployeeId.HasValue)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(x =>
                    x.Id == request.AssignedToEmployeeId.Value &&
                    x.CompanyId == companyId);

            if (!employeeExists)
            {
                throw new InvalidOperationException(
                    "Assigned employee does not belong to this company.");
            }
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.AssignedToEmployeeId = request.AssignedToEmployeeId;
        task.Status = request.Status.Trim();
        task.Priority = request.Priority.Trim();
        task.DueDate = request.DueDate;
        task.IsCompleted = request.IsCompleted;

        await _context.SaveChangesAsync();

        return new TaskResponse
        {
            Id = task.Id,
            CompanyId = task.CompanyId,
            AssignedToEmployeeId = task.AssignedToEmployeeId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(
        Guid companyId,
        Guid taskId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(x =>
                x.Id == taskId &&
                x.CompanyId == companyId);

        if (task is null)
        {
            return false;
        }

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();

        return true;
    }
}