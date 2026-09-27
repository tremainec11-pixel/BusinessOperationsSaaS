namespace BusinessOperationsSaaS.Application.Subscriptions.DTOs;

public class CreateSubscriptionDto
{
    public Guid SubscriptionPlanId { get; set; }
    public int TrialDays { get; set; } = 0;
}
