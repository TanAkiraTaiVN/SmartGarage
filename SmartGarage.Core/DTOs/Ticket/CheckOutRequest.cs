using SmartGarage.Core.Enums;

namespace SmartGarage.Core.DTOs.Ticket;

public class CheckOutRequest
{
    public string TicketCode { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
}
