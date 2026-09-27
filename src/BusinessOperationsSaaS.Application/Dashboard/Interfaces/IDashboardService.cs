using BusinessOperationsSaaS.Application.Dashboard.DTOs;

namespace BusinessOperationsSaaS.Application.Dashboard.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid companyId);
}