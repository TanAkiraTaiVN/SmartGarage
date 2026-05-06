using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarage.Core.DTOs;
using SmartGarage.Core.DTOs.Payment;
using SmartGarage.Core.Interfaces;

namespace SmartGarage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<PaymentDto>>>> GetAll()
    {
        var result = await _paymentService.GetAllAsync();
        return Ok(ApiResponse<List<PaymentDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> GetById(int id)
    {
        var result = await _paymentService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<PaymentDto>.Fail("Không tìm thấy giao dịch."));
        return Ok(ApiResponse<PaymentDto>.Ok(result));
    }

    [HttpGet("my-payments")]
    public async Task<ActionResult<ApiResponse<List<PaymentDto>>>> GetMyPayments()
    {
        var userId = GetUserId();
        var result = await _paymentService.GetByUserIdAsync(userId);
        return Ok(ApiResponse<List<PaymentDto>>.Ok(result));
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
