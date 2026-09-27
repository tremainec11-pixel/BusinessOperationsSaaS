namespace BusinessOperationsSaaS.Domain.Entities;

public class SubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }

    public int MaxEmployees { get; set; }
    public int MaxProducts { get; set; }
    public int MaxTasks { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
