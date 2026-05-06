namespace SmartGarage.Core.DTOs.Ticket;

public class CheckInRequest
{
    public int VehicleId { get; set; }
    public int ParkingSpotId { get; set; }
}
