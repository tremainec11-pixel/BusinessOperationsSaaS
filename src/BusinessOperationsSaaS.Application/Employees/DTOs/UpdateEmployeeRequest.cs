namespace BusinessOperationsSaaS.Application.Employees.DTOs;

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public string? Department { get; set; }

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; }
}
