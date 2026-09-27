namespace BusinessOperationsSaaS.Application.Financials.DTOs;

public class CreateFinancialTransactionRequest
{
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
}