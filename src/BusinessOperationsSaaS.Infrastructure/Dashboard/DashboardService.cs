using BusinessOperationsSaaS.Application.Dashboard.DTOs;
using BusinessOperationsSaaS.Application.Dashboard.Interfaces;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperationsSaaS.Infrastructure.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponse> GetDashboardAsync(
        Guid companyId)
    {
        var totalEmployees = await _context.Employees
            .CountAsync(x => x.CompanyId == companyId);

        var activeEmployees = await _context.Employees
            .CountAsync(x =>
                x.CompanyId == companyId &&
                x.IsActive);

        var totalTasks = await _context.Tasks
            .CountAsync(x => x.CompanyId == companyId);

        var pendingTasks = await _context.Tasks
            .CountAsync(x =>
                x.CompanyId == companyId &&
                !x.IsCompleted &&
                x.Status == "Pending");

        var completedTasks = await _context.Tasks
            .CountAsync(x =>
                x.CompanyId == companyId &&
                x.IsCompleted);

        var totalProducts = await _context.Products
            .CountAsync(x => x.CompanyId == companyId);

        var lowStockProducts = await _context.Products
            .CountAsync(x =>
                x.CompanyId == companyId &&
                x.IsActive &&
                x.Stock <= 5);

        var totalIncome = await _context.FinancialTransactions
            .Where(x =>
                x.CompanyId == companyId &&
                x.IsActive &&
                x.Type == "Income")
            .SumAsync(x => (decimal?)x.Amount) ?? 0;

        var totalExpenses = await _context.FinancialTransactions
            .Where(x =>
                x.CompanyId == companyId &&
                x.IsActive &&
                x.Type == "Expense")
            .SumAsync(x => (decimal?)x.Amount) ?? 0;

        return new DashboardResponse
        {
            TotalEmployees = totalEmployees,
            ActiveEmployees = activeEmployees,

            TotalTasks = totalTasks,
            PendingTasks = pendingTasks,
            CompletedTasks = completedTasks,

            TotalProducts = totalProducts,
            LowStockProducts = lowStockProducts,

            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = totalIncome - totalExpenses
        };
    }
}