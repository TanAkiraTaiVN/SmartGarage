namespace SmartGarage.Core.DTOs.Dashboard;

public class DashboardDto
{
    public int TotalSpots { get; set; }
    public int AvailableSpots { get; set; }
    public int OccupiedSpots { get; set; }
    public int TotalVehiclesParked { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal YearlyRevenue { get; set; }
    public List<RevenueByDay> RevenueByDays { get; set; } = new();
    public List<VehicleTypeStats> VehicleTypeStats { get; set; } = new();
}

public class RevenueByDay
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int TotalTickets { get; set; }
}

public class VehicleTypeStats
{
    public string VehicleType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Revenue { get; set; }
}
