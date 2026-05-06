using SmartGarage.Core.Enums;

namespace SmartGarage.Core.Entities;

public class ParkingRate
{
    public int Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
