using SmartGarage.Core.DTOs.Payment;

namespace SmartGarage.Core.Interfaces;

public interface IPaymentService
{
    Task<List<PaymentDto>> GetAllAsync();
    Task<PaymentDto?> GetByIdAsync(int id);
    Task<List<PaymentDto>> GetByUserIdAsync(int userId);
}
