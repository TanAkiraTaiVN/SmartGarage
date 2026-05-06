using SmartGarage.Core.DTOs.Vehicle;

namespace SmartGarage.Core.Interfaces;

public interface IVehicleService
{
    Task<List<VehicleDto>> GetAllAsync();
    Task<VehicleDto?> GetByIdAsync(int id);
    Task<List<VehicleDto>> GetByOwnerIdAsync(int ownerId);
    Task<VehicleDto> CreateAsync(int ownerId, CreateVehicleRequest request);
    Task<VehicleDto?> UpdateAsync(int id, UpdateVehicleRequest request);
    Task<bool> DeleteAsync(int id);
}
