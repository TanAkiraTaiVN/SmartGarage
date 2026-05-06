using SmartGarage.Core.Enums;

namespace SmartGarage.Core.DTOs.Vehicle;

public class UpdateVehicleRequest
{
    public string? LicensePlate { get; set; }
    public VehicleType? VehicleType { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Color { get; set; }
}
