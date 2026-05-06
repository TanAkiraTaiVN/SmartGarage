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

        var todayPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= today)
            .Select(p => p.Amount)
            .ToListAsync();
        var todayRevenue = todayPayments.Sum();

        var monthlyPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= monthStart)
            .Select(p => p.Amount)
            .ToListAsync();
        var monthlyRevenue = monthlyPayments.Sum();

        var yearlyPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= yearStart)
            .Select(p => p.Amount)
            .ToListAsync();
        var yearlyRevenue = yearlyPayments.Sum();

        var last7Days = today.AddDays(-6);
        var recentPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= last7Days)
            .Select(p => new { p.CompletedAt, p.Amount })
            .ToListAsync();

        var revenueByDays = recentPayments
            .GroupBy(p => p.CompletedAt!.Value.Date)
            .Select(g => new RevenueByDay
            {
                Date = g.Key,
                Revenue = g.Sum(p => p.Amount),
                TotalTickets = g.Count()
            })
            .OrderBy(r => r.Date)
            .ToList();

        var ticketsThisMonth = await _context.ParkingTickets
            .Include(t => t.Vehicle)
            .Where(t => t.CreatedAt >= monthStart)
            .Select(t => new { t.Vehicle.VehicleType, t.TotalAmount })
            .ToListAsync();

        var vehicleTypeStats = ticketsThisMonth
            .GroupBy(t => t.VehicleType)
            .Select(g => new VehicleTypeStats
            {
                VehicleType = g.Key.ToString(),
                Count = g.Count(),
                Revenue = g.Sum(t => t.TotalAmount ?? 0)
            })
            .ToList();

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
        var payments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed
                && p.CompletedAt >= from && p.CompletedAt <= to)
            .Select(p => new { p.CompletedAt, p.Amount })
            .ToListAsync();

        return payments
            .GroupBy(p => p.CompletedAt!.Value.Date)
            .Select(g => new RevenueByDay
            {
                Date = g.Key,
                Revenue = g.Sum(p => p.Amount),
                TotalTickets = g.Count()
            })
            .OrderBy(r => r.Date)
            .ToList();
    }
}
