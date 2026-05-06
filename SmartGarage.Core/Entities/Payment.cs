using SmartGarage.Core.Enums;

namespace SmartGarage.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public int ParkingTicketId { get; set; }
    public ParkingTicket ParkingTicket { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
