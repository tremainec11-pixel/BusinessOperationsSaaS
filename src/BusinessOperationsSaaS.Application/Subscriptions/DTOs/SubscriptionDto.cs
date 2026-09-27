namespace BusinessOperationsSaaS.Application.Subscriptions.DTOs;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public SubscriptionPlanDto SubscriptionPlan { get; set; } = null!;
}
