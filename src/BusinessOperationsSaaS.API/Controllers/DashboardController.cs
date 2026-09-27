using System.Security.Claims;
using BusinessOperationsSaaS.Application.Dashboard.DTOs;
using BusinessOperationsSaaS.Application.Dashboard.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> GetDashboard()
    {
        var companyIdClaim = User.FindFirst("companyId")?.Value;

        if (!Guid.TryParse(companyIdClaim, out var companyId))
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var dashboard = await _dashboardService.GetDashboardAsync(
            companyId);

        return Ok(dashboard);
    }
}