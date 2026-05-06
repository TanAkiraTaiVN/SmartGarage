using SmartGarage.Core.Enums;

namespace SmartGarage.Core.DTOs.ParkingSpot;

public class CreateParkingSpotRequest
{
    public string SpotCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public int Floor { get; set; }
    public VehicleType SpotType { get; set; }
}
