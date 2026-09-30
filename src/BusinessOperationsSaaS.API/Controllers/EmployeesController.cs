using BusinessOperationsSaaS.Application.Employees.DTOs;
using BusinessOperationsSaaS.Application.Employees.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponse>>> GetAll()
    {
        var companyId = PublicCompanyId;

        

        var employees = await _employeeService.GetAllAsync(
            companyId);

        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetById(Guid id)
    {
        var companyId = PublicCompanyId;

        

        var employee = await _employeeService.GetByIdAsync(
            companyId,
            id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create(
        CreateEmployeeRequest request)
    {
        var companyId = PublicCompanyId;

        

        try
        {
            var employee = await _employeeService.CreateAsync(
                companyId,
                request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = employee.Id },
                employee);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> Update(
        Guid id,
        UpdateEmployeeRequest request)
    {
        var companyId = PublicCompanyId;

        

        var employee = await _employeeService.UpdateAsync(
            companyId,
            id,
            request);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        return Ok(employee);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var companyId = PublicCompanyId;

        

        var deleted = await _employeeService.DeleteAsync(
            companyId,
            id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        return NoContent();
    }

    private static readonly Guid PublicCompanyId = Guid.Parse("feb88165-b70c-429e-83a3-94d412dca312");
}
