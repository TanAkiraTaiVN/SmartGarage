namespace SmartGarage.Core.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public string TicketCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
