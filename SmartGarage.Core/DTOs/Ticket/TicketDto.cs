namespace SmartGarage.Core.DTOs.Ticket;

public class TicketDto
{
    public int Id { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public string QrCodeBase64 { get; set; } = string.Empty;
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string ParkingSpotCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
}
