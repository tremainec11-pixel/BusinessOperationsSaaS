namespace BusinessOperationsSaaS.Application.Subscriptions.DTOs;

public class UpdateSubscriptionDto
{
    public Guid SubscriptionPlanId { get; set; }
    public string Status { get; set; } = "Active";
}
