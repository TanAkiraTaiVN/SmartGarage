using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarage.Core.DTOs;
using SmartGarage.Core.DTOs.Ticket;
using SmartGarage.Core.Interfaces;

namespace SmartGarage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<ActionResult<ApiResponse<List<TicketDto>>>> GetAll()
    {
        var result = await _ticketService.GetAllAsync();
        return Ok(ApiResponse<List<TicketDto>>.Ok(result));
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<ActionResult<ApiResponse<List<TicketDto>>>> GetActive()
    {
        var result = await _ticketService.GetActiveAsync();
        return Ok(ApiResponse<List<TicketDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> GetById(int id)
    {
        var result = await _ticketService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<TicketDto>.Fail("Không tìm thấy vé xe."));
        return Ok(ApiResponse<TicketDto>.Ok(result));
    }

    [HttpGet("code/{ticketCode}")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> GetByCode(string ticketCode)
    {
        var result = await _ticketService.GetByTicketCodeAsync(ticketCode);
        if (result == null)
            return NotFound(ApiResponse<TicketDto>.Fail("Không tìm thấy vé xe."));
        return Ok(ApiResponse<TicketDto>.Ok(result));
    }

    [HttpGet("my-tickets")]
    public async Task<ActionResult<ApiResponse<List<TicketDto>>>> GetMyTickets()
    {
        var userId = GetUserId();
        var result = await _ticketService.GetByUserIdAsync(userId);
        return Ok(ApiResponse<List<TicketDto>>.Ok(result));
    }

    [HttpPost("check-in")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> CheckIn([FromBody] CheckInRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _ticketService.CheckInAsync(userId, request);
            return Ok(ApiResponse<TicketDto>.Ok(result, "Check-in thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TicketDto>.Fail(ex.Message));
        }
    }

    [HttpPost("check-out")]
    public async Task<ActionResult<ApiResponse<TicketDto>>> CheckOut([FromBody] CheckOutRequest request)
    {
        try
        {
            var result = await _ticketService.CheckOutAsync(request);
            return Ok(ApiResponse<TicketDto>.Ok(result, "Check-out thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TicketDto>.Fail(ex.Message));
        }
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
