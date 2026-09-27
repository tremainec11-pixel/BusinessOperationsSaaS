namespace BusinessOperationsSaaS.Application.Employees.DTOs;

public class CreateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Position { get; set; }

    public string? Department { get; set; }

    public DateTime HireDate { get; set; }
}