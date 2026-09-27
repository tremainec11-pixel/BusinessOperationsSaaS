namespace BusinessOperationsSaaS.Domain.Entities;

public class FinancialTransaction
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }

    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
}