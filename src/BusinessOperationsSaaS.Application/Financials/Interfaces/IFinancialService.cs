using BusinessOperationsSaaS.Application.Financials.DTOs;

namespace BusinessOperationsSaaS.Application.Financials.Interfaces;

public interface IFinancialService
{
    Task<List<FinancialTransactionResponse>> GetAllAsync(
        Guid companyId);

    Task<FinancialTransactionResponse?> GetByIdAsync(
        Guid companyId,
        Guid transactionId);

    Task<FinancialTransactionResponse> CreateAsync(
        Guid companyId,
        CreateFinancialTransactionRequest request);

    Task<FinancialTransactionResponse?> UpdateAsync(
        Guid companyId,
        Guid transactionId,
        UpdateFinancialTransactionRequest request);

    Task<bool> DeleteAsync(
        Guid companyId,
        Guid transactionId);
}