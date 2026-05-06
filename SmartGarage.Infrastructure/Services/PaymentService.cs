using Microsoft.EntityFrameworkCore;
using SmartGarage.Core.DTOs.Payment;
using SmartGarage.Core.Interfaces;
using SmartGarage.Infrastructure.Data;

namespace SmartGarage.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly SmartGarageDbContext _context;

    public PaymentService(SmartGarageDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentDto>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.ParkingTicket).ThenInclude(t => t.Vehicle)
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                TransactionCode = p.TransactionCode,
                TicketCode = p.ParkingTicket.TicketCode,
                CustomerName = p.User.FullName,
                VehicleLicensePlate = p.ParkingTicket.Vehicle.LicensePlate,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod.ToString(),
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt,
                CompletedAt = p.CompletedAt
            })
            .ToListAsync();
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        return await _context.Payments
            .Include(p => p.ParkingTicket).ThenInclude(t => t.Vehicle)
            .Include(p => p.User)
            .Where(p => p.Id == id)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                TransactionCode = p.TransactionCode,
                TicketCode = p.ParkingTicket.TicketCode,
                CustomerName = p.User.FullName,
                VehicleLicensePlate = p.ParkingTicket.Vehicle.LicensePlate,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod.ToString(),
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt,
                CompletedAt = p.CompletedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<PaymentDto>> GetByUserIdAsync(int userId)
    {
        return await _context.Payments
            .Include(p => p.ParkingTicket).ThenInclude(t => t.Vehicle)
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                TransactionCode = p.TransactionCode,
                TicketCode = p.ParkingTicket.TicketCode,
                CustomerName = p.User.FullName,
                VehicleLicensePlate = p.ParkingTicket.Vehicle.LicensePlate,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod.ToString(),
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt,
                CompletedAt = p.CompletedAt
            })
            .ToListAsync();
    }
}
