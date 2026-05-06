using SmartGarage.Core.Enums;

namespace SmartGarage.Core.Entities;

public class ParkingTicket
{
    public int Id { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public string QrCode { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public int ParkingSpotId { get; set; }
    public ParkingSpot ParkingSpot { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public DateTime? CheckOutTime { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Active;
    public decimal? TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Payment? Payment { get; set; }
}
