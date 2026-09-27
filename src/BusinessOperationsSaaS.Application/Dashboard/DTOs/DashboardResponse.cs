namespace BusinessOperationsSaaS.Application.Dashboard.DTOs;

public class DashboardResponse
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }

    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int CompletedTasks { get; set; }

    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }

    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
}