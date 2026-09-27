using BusinessOperationsSaaS.Application.Employees.DTOs;

namespace BusinessOperationsSaaS.Application.Employees.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeResponse>> GetAllAsync(Guid companyId);

    Task<EmployeeResponse?> GetByIdAsync(
        Guid companyId,
        Guid employeeId);

    Task<EmployeeResponse> CreateAsync(
        Guid companyId,
        CreateEmployeeRequest request);

    Task<EmployeeResponse?> UpdateAsync(
        Guid companyId,
        Guid employeeId,
        UpdateEmployeeRequest request);

    Task<bool> DeleteAsync(
        Guid companyId,
        Guid employeeId);
}