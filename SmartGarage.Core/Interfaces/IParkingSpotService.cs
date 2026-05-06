using SmartGarage.Core.DTOs.ParkingSpot;
using SmartGarage.Core.Enums;

namespace SmartGarage.Core.Interfaces;

public interface IParkingSpotService
{
    Task<List<ParkingSpotDto>> GetAllAsync();
    Task<List<ParkingSpotDto>> GetAvailableAsync(VehicleType? vehicleType = null);
    Task<ParkingSpotDto?> GetByIdAsync(int id);
    Task<ParkingSpotDto> CreateAsync(CreateParkingSpotRequest request);
    Task<bool> UpdateStatusAsync(int id, ParkingSpotStatus status);
    Task<bool> DeleteAsync(int id);
}
