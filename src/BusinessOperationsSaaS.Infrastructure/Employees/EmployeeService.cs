using BusinessOperationsSaaS.Application.Employees.DTOs;
using BusinessOperationsSaaS.Application.Employees.Interfaces;
using BusinessOperationsSaaS.Application.Subscriptions.Services;
using BusinessOperationsSaaS.Domain.Entities;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperationsSaaS.Infrastructure.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly ISubscriptionLimitService _subscriptionLimitService;

    public EmployeeService(
        AppDbContext context,
        ISubscriptionLimitService subscriptionLimitService)
    {
        _context = context;
        _subscriptionLimitService = subscriptionLimitService;
    }

    public async Task<List<EmployeeResponse>> GetAllAsync(Guid companyId)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new EmployeeResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Position = x.Position,
                Department = x.Department,
                HireDate = x.HireDate,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<EmployeeResponse?> GetByIdAsync(
        Guid companyId,
        Guid employeeId)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(x =>
                x.Id == employeeId &&
                x.CompanyId == companyId)
            .Select(x => new EmployeeResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Position = x.Position,
                Department = x.Department,
                HireDate = x.HireDate,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EmployeeResponse> CreateAsync(
        Guid companyId,
        CreateEmployeeRequest request)
    {
        var canCreate =
            await _subscriptionLimitService
                .CanCreateEmployeeAsync(companyId);

        if (!canCreate)
        {
            throw new InvalidOperationException(
                "Employee limit reached for the current subscription plan.");
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Phone = request.Phone?.Trim(),
            Position = request.Position?.Trim(),
            Department = request.Department?.Trim(),
            HireDate = DateTime.SpecifyKind(
                request.HireDate,
                DateTimeKind.Utc),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        return new EmployeeResponse
        {
            Id = employee.Id,
            CompanyId = employee.CompanyId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Position = employee.Position,
            Department = employee.Department,
            HireDate = employee.HireDate,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt
        };
    }

    public async Task<EmployeeResponse?> UpdateAsync(
        Guid companyId,
        Guid employeeId,
        UpdateEmployeeRequest request)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(x =>
                x.Id == employeeId &&
                x.CompanyId == companyId);

        if (employee is null)
        {
            return null;
        }

        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = request.Email.Trim().ToLowerInvariant();
        employee.Phone = request.Phone?.Trim();
        employee.Position = request.Position?.Trim();
        employee.Department = request.Department?.Trim();
        employee.HireDate = request.HireDate;
        employee.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return new EmployeeResponse
        {
            Id = employee.Id,
            CompanyId = employee.CompanyId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Position = employee.Position,
            Department = employee.Department,
            HireDate = employee.HireDate,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(
        Guid companyId,
        Guid employeeId)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(x =>
                x.Id == employeeId &&
                x.CompanyId == companyId);

        if (employee is null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return true;
    }
}

