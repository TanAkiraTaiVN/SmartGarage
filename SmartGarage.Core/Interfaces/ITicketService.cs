using SmartGarage.Core.DTOs.Ticket;

namespace SmartGarage.Core.Interfaces;

public interface ITicketService
{
    Task<List<TicketDto>> GetAllAsync();
    Task<List<TicketDto>> GetActiveAsync();
    Task<TicketDto?> GetByIdAsync(int id);
    Task<TicketDto?> GetByTicketCodeAsync(string ticketCode);
    Task<List<TicketDto>> GetByUserIdAsync(int userId);
    Task<TicketDto> CheckInAsync(int userId, CheckInRequest request);
    Task<TicketDto> CheckOutAsync(CheckOutRequest request);
}
