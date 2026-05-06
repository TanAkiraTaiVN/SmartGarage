using Microsoft.EntityFrameworkCore;
using QRCoder;
using SmartGarage.Core.DTOs.Ticket;
using SmartGarage.Core.Entities;
using SmartGarage.Core.Enums;
using SmartGarage.Core.Interfaces;
using SmartGarage.Infrastructure.Data;

namespace SmartGarage.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly SmartGarageDbContext _context;
    private readonly INotificationService _notificationService;

    public TicketService(SmartGarageDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<List<TicketDto>> GetAllAsync()
    {
        return await _context.ParkingTickets
            .Include(t => t.Vehicle)
            .Include(t => t.ParkingSpot)
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task<List<TicketDto>> GetActiveAsync()
    {
        return await _context.ParkingTickets
            .Include(t => t.Vehicle)
            .Include(t => t.ParkingSpot)
            .Include(t => t.User)
            .Where(t => t.Status == TicketStatus.Active)
            .OrderByDescending(t => t.CheckInTime)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task<TicketDto?> GetByIdAsync(int id)
    {
        var ticket = await _context.ParkingTickets
            .Include(t => t.Vehicle)
            .Include(t => t.ParkingSpot)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);

        return ticket == null ? null : MapToDto(ticket);
    }

    public async Task<TicketDto?> GetByTicketCodeAsync(string ticketCode)
    {
        var ticket = await _context.ParkingTickets
            .Include(t => t.Vehicle)
            .Include(t => t.ParkingSpot)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TicketCode == ticketCode);

        return ticket == null ? null : MapToDto(ticket);
    }

    public async Task<List<TicketDto>> GetByUserIdAsync(int userId)
    {
        return await _context.ParkingTickets
            .Include(t => t.Vehicle)
            .Include(t => t.ParkingSpot)
            .Include(t => t.User)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task<TicketDto> CheckInAsync(int userId, CheckInRequest request)
    {
        var vehicle = await _context.Vehicles.FindAsync(request.VehicleId)
            ?? throw new InvalidOperationException("Phương tiện không tồn tại.");

        var spot = await _context.ParkingSpots.FindAsync(request.ParkingSpotId)
            ?? throw new InvalidOperationException("Vị trí đỗ xe không tồn tại.");

        if (spot.Status != ParkingSpotStatus.Available)
            throw new InvalidOperationException("Vị trí đỗ xe không trống.");

        if (spot.SpotType != vehicle.VehicleType)
            throw new InvalidOperationException("Loại phương tiện không phù hợp với vị trí đỗ.");

        var hasActiveTicket = await _context.ParkingTickets
            .AnyAsync(t => t.VehicleId == request.VehicleId && t.Status == TicketStatus.Active);
        if (hasActiveTicket)
            throw new InvalidOperationException("Phương tiện đang có vé xe hoạt động.");

        var ticketCode = $"TK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        var qrCode = GenerateQrCode(ticketCode);

        var ticket = new ParkingTicket
        {
            TicketCode = ticketCode,
            QrCode = qrCode,
            VehicleId = request.VehicleId,
            ParkingSpotId = request.ParkingSpotId,
            UserId = userId,
            CheckInTime = DateTime.UtcNow,
            Status = TicketStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        spot.Status = ParkingSpotStatus.Occupied;
        spot.UpdatedAt = DateTime.UtcNow;

        _context.ParkingTickets.Add(ticket);
        await _context.SaveChangesAsync();

        await _notificationService.SendAsync(userId,
            "Check-in thành công",
            $"Xe {vehicle.LicensePlate} đã được gửi tại vị trí {spot.SpotCode}. Mã vé: {ticketCode}");

        await _context.Entry(ticket).Reference(t => t.Vehicle).LoadAsync();
        await _context.Entry(ticket).Reference(t => t.ParkingSpot).LoadAsync();
        await _context.Entry(ticket).Reference(t => t.User).LoadAsync();

        return MapToDto(ticket);
    }

    public async Task<TicketDto> CheckOutAsync(CheckOutRequest request)
    {
        var ticket = await _context.ParkingTickets
            .Include(t => t.Vehicle)
            .Include(t => t.ParkingSpot)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TicketCode == request.TicketCode && t.Status == TicketStatus.Active)
            ?? throw new InvalidOperationException("Vé xe không hợp lệ hoặc đã checkout.");

        ticket.CheckOutTime = DateTime.UtcNow;
        ticket.Status = TicketStatus.CheckedOut;

        var duration = ticket.CheckOutTime.Value - ticket.CheckInTime;
        var rate = await _context.ParkingRates
            .FirstOrDefaultAsync(r => r.VehicleType == ticket.Vehicle.VehicleType && r.IsActive)
            ?? throw new InvalidOperationException("Không tìm thấy bảng giá cho loại xe này.");

        decimal totalAmount;
        if (duration.TotalDays >= 1)
            totalAmount = (decimal)Math.Ceiling(duration.TotalDays) * rate.DailyRate;
        else
            totalAmount = (decimal)Math.Ceiling(duration.TotalHours) * rate.HourlyRate;

        ticket.TotalAmount = totalAmount;

        var payment = new Payment
        {
            TransactionCode = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            ParkingTicketId = ticket.Id,
            UserId = ticket.UserId,
            Amount = totalAmount,
            PaymentMethod = request.PaymentMethod,
            Status = PaymentStatus.Completed,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        ticket.ParkingSpot.Status = ParkingSpotStatus.Available;
        ticket.ParkingSpot.UpdatedAt = DateTime.UtcNow;

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        await _notificationService.SendAsync(ticket.UserId,
            "Check-out thành công",
            $"Xe {ticket.Vehicle.LicensePlate} đã check-out. Tổng phí: {totalAmount:N0} VND");

        return MapToDto(ticket);
    }

    private static string GenerateQrCode(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(10);
        return Convert.ToBase64String(qrCodeBytes);
    }

    private static TicketDto MapToDto(ParkingTicket t) => new()
    {
        Id = t.Id,
        TicketCode = t.TicketCode,
        QrCodeBase64 = t.QrCode,
        VehicleLicensePlate = t.Vehicle?.LicensePlate ?? string.Empty,
        VehicleType = t.Vehicle?.VehicleType.ToString() ?? string.Empty,
        ParkingSpotCode = t.ParkingSpot?.SpotCode ?? string.Empty,
        Zone = t.ParkingSpot?.Zone ?? string.Empty,
        Floor = t.ParkingSpot?.Floor ?? 0,
        CustomerName = t.User?.FullName ?? string.Empty,
        CheckInTime = t.CheckInTime,
        CheckOutTime = t.CheckOutTime,
        Status = t.Status.ToString(),
        TotalAmount = t.TotalAmount
    };
}
