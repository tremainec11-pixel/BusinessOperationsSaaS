using System.Security.Claims;
using BusinessOperationsSaaS.Application.Tasks.DTOs;
using BusinessOperationsSaaS.Application.Tasks.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskResponse>>> GetAll()
    {
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var tasks = await _taskService.GetAllAsync(companyId.Value);

        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id)
    {
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var task = await _taskService.GetByIdAsync(
            companyId.Value,
            id);

        if (task is null)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(
        CreateTaskRequest request)
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
            var task = await _taskService.CreateAsync(
                companyId.Value,
                request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = task.Id },
                task);
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
    public async Task<ActionResult<TaskResponse>> Update(
        Guid id,
        UpdateTaskRequest request)
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
            var task = await _taskService.UpdateAsync(
                companyId.Value,
                id,
                request);

            if (task is null)
            {
                return NotFound(new
                {
                    message = "Task not found."
                });
            }

            return Ok(task);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
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

        var deleted = await _taskService.DeleteAsync(
            companyId.Value,
            id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Task not found."
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
