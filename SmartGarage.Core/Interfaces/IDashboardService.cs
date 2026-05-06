using SmartGarage.Core.DTOs.Dashboard;

namespace SmartGarage.Core.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
    Task<List<RevenueByDay>> GetRevenueByDateRangeAsync(DateTime from, DateTime to);
}
