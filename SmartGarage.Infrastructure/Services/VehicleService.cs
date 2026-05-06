using Microsoft.EntityFrameworkCore;
using SmartGarage.Core.DTOs.Vehicle;
using SmartGarage.Core.Entities;
using SmartGarage.Core.Interfaces;
using SmartGarage.Infrastructure.Data;

namespace SmartGarage.Infrastructure.Services;

public class VehicleService : IVehicleService
{
    private readonly SmartGarageDbContext _context;

    public VehicleService(SmartGarageDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleDto>> GetAllAsync()
    {
        return await _context.Vehicles
            .Include(v => v.Owner)
            .Where(v => v.IsActive)
            .Select(v => MapToDto(v))
            .ToListAsync();
    }

    public async Task<VehicleDto?> GetByIdAsync(int id)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Owner)
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        return vehicle == null ? null : MapToDto(vehicle);
    }

    public async Task<List<VehicleDto>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Vehicles
            .Include(v => v.Owner)
            .Where(v => v.OwnerId == ownerId && v.IsActive)
            .Select(v => MapToDto(v))
            .ToListAsync();
    }

    public async Task<VehicleDto> CreateAsync(int ownerId, CreateVehicleRequest request)
    {
        if (await _context.Vehicles.AnyAsync(v => v.LicensePlate == request.LicensePlate && v.IsActive))
            throw new InvalidOperationException("Biển số xe đã tồn tại trong hệ thống.");

        var vehicle = new Vehicle
        {
            LicensePlate = request.LicensePlate,
            VehicleType = request.VehicleType,
            Brand = request.Brand,
            Model = request.Model,
            Color = request.Color,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        await _context.Entry(vehicle).Reference(v => v.Owner).LoadAsync();
        return MapToDto(vehicle);
    }

    public async Task<VehicleDto?> UpdateAsync(int id, UpdateVehicleRequest request)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Owner)
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        if (vehicle == null) return null;

        if (request.LicensePlate != null) vehicle.LicensePlate = request.LicensePlate;
        if (request.VehicleType.HasValue) vehicle.VehicleType = request.VehicleType.Value;
        if (request.Brand != null) vehicle.Brand = request.Brand;
        if (request.Model != null) vehicle.Model = request.Model;
        if (request.Color != null) vehicle.Color = request.Color;

        await _context.SaveChangesAsync();
        return MapToDto(vehicle);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return false;

        vehicle.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static VehicleDto MapToDto(Vehicle v) => new()
    {
        Id = v.Id,
        LicensePlate = v.LicensePlate,
        VehicleType = v.VehicleType.ToString(),
        Brand = v.Brand,
        Model = v.Model,
        Color = v.Color,
        OwnerName = v.Owner?.FullName ?? string.Empty,
        CreatedAt = v.CreatedAt
    };
}
