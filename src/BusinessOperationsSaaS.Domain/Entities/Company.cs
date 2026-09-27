namespace BusinessOperationsSaaS.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Industry { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}

