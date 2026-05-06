namespace SmartGarage.Core.DTOs.ParkingSpot;

public class ParkingSpotDto
{
    public int Id { get; set; }
    public string SpotCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string SpotType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
