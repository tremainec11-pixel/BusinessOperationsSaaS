using BusinessOperationsSaaS.Application.Tasks.DTOs;
using BusinessOperationsSaaS.Application.Tasks.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var companyId = PublicCompanyId;

        

        var tasks = await _taskService.GetAllAsync(companyId);

        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id)
    {
        var companyId = PublicCompanyId;

        

        var task = await _taskService.GetByIdAsync(
            companyId,
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
        var companyId = PublicCompanyId;

        

        try
        {
            var task = await _taskService.CreateAsync(
                companyId,
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
        var companyId = PublicCompanyId;

        

        try
        {
            var task = await _taskService.UpdateAsync(
                companyId,
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
        var companyId = PublicCompanyId;

        

        var deleted = await _taskService.DeleteAsync(
            companyId,
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

    private static readonly Guid PublicCompanyId = Guid.Parse("feb88165-b70c-429e-83a3-94d412dca312");
}

