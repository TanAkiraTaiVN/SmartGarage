using Microsoft.AspNetCore.Mvc;
using SmartGarage.Core.DTOs;
using SmartGarage.Core.DTOs.Auth;
using SmartGarage.Core.Interfaces;

namespace SmartGarage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(result, "Đăng nhập thành công."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponse>.Fail(ex.Message));
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(result, "Đăng ký thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<AuthResponse>.Fail(ex.Message));
        }
    }
}
