namespace BusinessOperationsSaaS.Application.Tasks.DTOs;

public class TaskResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? AssignedToEmployeeId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
