namespace BusinessOperationsSaaS.Domain.Entities;

public class Task
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? AssignedToEmployeeId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string Status { get; set; } = "Pending";
    public string Priority { get; set; } = "Medium";

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
    public Employee? AssignedToEmployee { get; set; }
}