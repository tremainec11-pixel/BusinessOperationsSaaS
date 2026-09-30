using BusinessOperationsSaaS.Application.Dashboard.DTOs;
using BusinessOperationsSaaS.Application.Dashboard.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    // Public company context
    private static readonly Guid PublicCompanyId =
        Guid.Parse("feb88165-b70c-429e-83a3-94d412dca312");

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> GetDashboard()
    {
        var dashboard = await _dashboardService.GetDashboardAsync(
            PublicCompanyId);

        return Ok(dashboard);
    }
}