using SmartGarage.Core.Enums;

namespace SmartGarage.Core.DTOs.Vehicle;

public class CreateVehicleRequest
{
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
