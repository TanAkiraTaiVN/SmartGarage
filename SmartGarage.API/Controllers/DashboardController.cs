using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarage.Core.DTOs;
using SmartGarage.Core.DTOs.Dashboard;
using SmartGarage.Core.Interfaces;

namespace SmartGarage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> GetDashboard()
    {
        var result = await _dashboardService.GetDashboardAsync();
        return Ok(ApiResponse<DashboardDto>.Ok(result));
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<ApiResponse<List<RevenueByDay>>>> GetRevenue(
        [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await _dashboardService.GetRevenueByDateRangeAsync(from, to);
        return Ok(ApiResponse<List<RevenueByDay>>.Ok(result));
    }
}
