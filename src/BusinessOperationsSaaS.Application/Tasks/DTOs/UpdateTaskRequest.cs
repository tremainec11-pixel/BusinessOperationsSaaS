namespace BusinessOperationsSaaS.Application.Tasks.DTOs;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? AssignedToEmployeeId { get; set; }
    public string Status { get; set; } = "Pending";
    public string Priority { get; set; } = "Medium";
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
}
