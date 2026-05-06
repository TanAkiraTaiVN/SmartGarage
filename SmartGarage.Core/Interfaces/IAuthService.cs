using SmartGarage.Core.DTOs.Auth;

namespace SmartGarage.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
}
