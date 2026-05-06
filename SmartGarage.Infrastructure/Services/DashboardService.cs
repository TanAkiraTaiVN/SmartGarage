using Microsoft.EntityFrameworkCore;
using SmartGarage.Core.DTOs.Dashboard;
using SmartGarage.Core.Enums;
using SmartGarage.Core.Interfaces;
using SmartGarage.Infrastructure.Data;

namespace SmartGarage.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly SmartGarageDbContext _context;

    public DashboardService(SmartGarageDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var yearStart = new DateTime(today.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalSpots = await _context.ParkingSpots.CountAsync();
        var availableSpots = await _context.ParkingSpots
            .CountAsync(s => s.Status == ParkingSpotStatus.Available);
        var occupiedSpots = await _context.ParkingSpots
            .CountAsync(s => s.Status == ParkingSpotStatus.Occupied);
        var totalVehiclesParked = await _context.ParkingTickets
            .CountAsync(t => t.Status == TicketStatus.Active);

        var todayRevenue = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= today)
            .SumAsync(p => (decimal?)p.Amount) ?? 0;

        var monthlyRevenue = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= monthStart)
            .SumAsync(p => (decimal?)p.Amount) ?? 0;

        var yearlyRevenue = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= yearStart)
            .SumAsync(p => (decimal?)p.Amount) ?? 0;

        var last7Days = today.AddDays(-6);
        var revenueByDays = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= last7Days)
            .GroupBy(p => p.CompletedAt!.Value.Date)
            .Select(g => new RevenueByDay
            {
                Date = g.Key,
                Revenue = g.Sum(p => p.Amount),
                TotalTickets = g.Count()
            })
            .OrderBy(r => r.Date)
            .ToListAsync();

        var vehicleTypeStats = await _context.ParkingTickets
            .Include(t => t.Vehicle)
            .Where(t => t.CreatedAt >= monthStart)
            .GroupBy(t => t.Vehicle.VehicleType)
            .Select(g => new VehicleTypeStats
            {
                VehicleType = g.Key.ToString(),
                Count = g.Count(),
                Revenue = g.Sum(t => t.TotalAmount ?? 0)
            })
            .ToListAsync();

        return new DashboardDto
        {
            TotalSpots = totalSpots,
            AvailableSpots = availableSpots,
            OccupiedSpots = occupiedSpots,
            TotalVehiclesParked = totalVehiclesParked,
            TodayRevenue = todayRevenue,
            MonthlyRevenue = monthlyRevenue,
            YearlyRevenue = yearlyRevenue,
            RevenueByDays = revenueByDays,
            VehicleTypeStats = vehicleTypeStats
        };
    }

    public async Task<List<RevenueByDay>> GetRevenueByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed
                && p.CompletedAt >= from && p.CompletedAt <= to)
            .GroupBy(p => p.CompletedAt!.Value.Date)
            .Select(g => new RevenueByDay
            {
                Date = g.Key,
                Revenue = g.Sum(p => p.Amount),
                TotalTickets = g.Count()
            })
            .OrderBy(r => r.Date)
            .ToListAsync();
    }
}
