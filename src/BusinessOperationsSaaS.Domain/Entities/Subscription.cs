namespace BusinessOperationsSaaS.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }
    public Guid SubscriptionPlanId { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }

    public DateTime? TrialEndsAt { get; set; }

    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
}
