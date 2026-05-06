using SmartGarage.Core.Enums;

namespace SmartGarage.Core.Entities;

public class ParkingSpot
{
    public int Id { get; set; }
    public string SpotCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public int Floor { get; set; }
    public VehicleType SpotType { get; set; }
    public ParkingSpotStatus Status { get; set; } = ParkingSpotStatus.Available;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ParkingTicket> ParkingTickets { get; set; } = new List<ParkingTicket>();
}
