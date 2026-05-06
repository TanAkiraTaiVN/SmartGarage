using Microsoft.EntityFrameworkCore;
using SmartGarage.Core.DTOs.ParkingSpot;
using SmartGarage.Core.Entities;
using SmartGarage.Core.Enums;
using SmartGarage.Core.Interfaces;
using SmartGarage.Infrastructure.Data;

namespace SmartGarage.Infrastructure.Services;

public class ParkingSpotService : IParkingSpotService
{
    private readonly SmartGarageDbContext _context;

    public ParkingSpotService(SmartGarageDbContext context)
    {
        _context = context;
    }

    public async Task<List<ParkingSpotDto>> GetAllAsync()
    {
        return await _context.ParkingSpots
            .OrderBy(s => s.Zone).ThenBy(s => s.Floor).ThenBy(s => s.SpotCode)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<List<ParkingSpotDto>> GetAvailableAsync(VehicleType? vehicleType = null)
    {
        var query = _context.ParkingSpots
            .Where(s => s.Status == ParkingSpotStatus.Available);

        if (vehicleType.HasValue)
            query = query.Where(s => s.SpotType == vehicleType.Value);

        return await query
            .OrderBy(s => s.Zone).ThenBy(s => s.Floor).ThenBy(s => s.SpotCode)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<ParkingSpotDto?> GetByIdAsync(int id)
    {
        var spot = await _context.ParkingSpots.FindAsync(id);
        return spot == null ? null : MapToDto(spot);
    }

    public async Task<ParkingSpotDto> CreateAsync(CreateParkingSpotRequest request)
    {
        if (await _context.ParkingSpots.AnyAsync(s => s.SpotCode == request.SpotCode))
            throw new InvalidOperationException("Mã vị trí đỗ xe đã tồn tại.");

        var spot = new ParkingSpot
        {
            SpotCode = request.SpotCode,
            Zone = request.Zone,
            Floor = request.Floor,
            SpotType = request.SpotType,
            Status = ParkingSpotStatus.Available,
            CreatedAt = DateTime.UtcNow
        };

        _context.ParkingSpots.Add(spot);
        await _context.SaveChangesAsync();
        return MapToDto(spot);
    }

    public async Task<bool> UpdateStatusAsync(int id, ParkingSpotStatus status)
    {
        var spot = await _context.ParkingSpots.FindAsync(id);
        if (spot == null) return false;

        spot.Status = status;
        spot.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var spot = await _context.ParkingSpots.FindAsync(id);
        if (spot == null) return false;

        _context.ParkingSpots.Remove(spot);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ParkingSpotDto MapToDto(ParkingSpot s) => new()
    {
        Id = s.Id,
        SpotCode = s.SpotCode,
        Zone = s.Zone,
        Floor = s.Floor,
        SpotType = s.SpotType.ToString(),
        Status = s.Status.ToString()
    };
}
