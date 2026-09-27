using System.Security.Claims;
using BusinessOperationsSaaS.Application.Employees.DTOs;
using BusinessOperationsSaaS.Application.Employees.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var employees = await _employeeService.GetAllAsync(
            companyId.Value);

        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetById(Guid id)
    {
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var employee = await _employeeService.GetByIdAsync(
            companyId.Value,
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
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        try
        {
            var employee = await _employeeService.CreateAsync(
                companyId.Value,
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
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var employee = await _employeeService.UpdateAsync(
            companyId.Value,
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
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var deleted = await _employeeService.DeleteAsync(
            companyId.Value,
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

    private Guid? GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("companyId")?.Value;

        if (Guid.TryParse(companyIdClaim, out var companyId))
        {
            return companyId;
        }

        return null;
    }
}