namespace BusinessOperationsSaaS.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public Guid? UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public string? Department { get; set; }

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;

    public User? User { get; set; }
}
