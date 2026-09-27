using BusinessOperationsSaaS.Application.Financials.DTOs;
using BusinessOperationsSaaS.Application.Financials.Interfaces;
using BusinessOperationsSaaS.Domain.Entities;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperationsSaaS.Infrastructure.Financials;

public class FinancialService : IFinancialService
{
    private readonly AppDbContext _context;

    public FinancialService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FinancialTransactionResponse>> GetAllAsync(
        Guid companyId)
    {
        return await _context.FinancialTransactions
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new FinancialTransactionResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                Type = x.Type,
                Category = x.Category,
                Description = x.Description,
                Amount = x.Amount,
                TransactionDate = x.TransactionDate,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<FinancialTransactionResponse?> GetByIdAsync(
        Guid companyId,
        Guid transactionId)
    {
        return await _context.FinancialTransactions
            .AsNoTracking()
            .Where(x =>
                x.Id == transactionId &&
                x.CompanyId == companyId)
            .Select(x => new FinancialTransactionResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                Type = x.Type,
                Category = x.Category,
                Description = x.Description,
                Amount = x.Amount,
                TransactionDate = x.TransactionDate,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<FinancialTransactionResponse> CreateAsync(
        Guid companyId,
        CreateFinancialTransactionRequest request)
    {
        var type = request.Type.Trim();

        if (type != "Income" && type != "Expense")
        {
            throw new InvalidOperationException(
                "Transaction type must be Income or Expense.");
        }

        if (request.Amount <= 0)
        {
            throw new InvalidOperationException(
                "Transaction amount must be greater than zero.");
        }

        var transaction = new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Type = type,
            Category = request.Category.Trim(),
            Description = request.Description.Trim(),
            Amount = request.Amount,
            TransactionDate = DateTime.SpecifyKind(request.TransactionDate, DateTimeKind.Utc),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.FinancialTransactions.Add(transaction);

        await _context.SaveChangesAsync();

        return new FinancialTransactionResponse
        {
            Id = transaction.Id,
            CompanyId = transaction.CompanyId,
            Type = transaction.Type,
            Category = transaction.Category,
            Description = transaction.Description,
            Amount = transaction.Amount,
            TransactionDate = transaction.TransactionDate,
            IsActive = transaction.IsActive,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task<FinancialTransactionResponse?> UpdateAsync(
        Guid companyId,
        Guid transactionId,
        UpdateFinancialTransactionRequest request)
    {
        var transaction = await _context.FinancialTransactions
            .FirstOrDefaultAsync(x =>
                x.Id == transactionId &&
                x.CompanyId == companyId);

        if (transaction is null)
        {
            return null;
        }

        var type = request.Type.Trim();

        if (type != "Income" && type != "Expense")
        {
            throw new InvalidOperationException(
                "Transaction type must be Income or Expense.");
        }

        if (request.Amount <= 0)
        {
            throw new InvalidOperationException(
                "Transaction amount must be greater than zero.");
        }

        transaction.Type = type;
        transaction.Category = request.Category.Trim();
        transaction.Description = request.Description.Trim();
        transaction.Amount = request.Amount;
        transaction.TransactionDate = request.TransactionDate;
        transaction.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return new FinancialTransactionResponse
        {
            Id = transaction.Id,
            CompanyId = transaction.CompanyId,
            Type = transaction.Type,
            Category = transaction.Category,
            Description = transaction.Description,
            Amount = transaction.Amount,
            TransactionDate = transaction.TransactionDate,
            IsActive = transaction.IsActive,
            CreatedAt = transaction.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(
        Guid companyId,
        Guid transactionId)
    {
        var transaction = await _context.FinancialTransactions
            .FirstOrDefaultAsync(x =>
                x.Id == transactionId &&
                x.CompanyId == companyId);

        if (transaction is null)
        {
            return false;
        }

        _context.FinancialTransactions.Remove(transaction);

        await _context.SaveChangesAsync();

        return true;
    }
}
