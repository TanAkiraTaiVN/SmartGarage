using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarage.Core.DTOs;
using SmartGarage.Core.DTOs.ParkingSpot;
using SmartGarage.Core.Enums;
using SmartGarage.Core.Interfaces;

namespace SmartGarage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ParkingSpotsController : ControllerBase
{
    private readonly IParkingSpotService _parkingSpotService;

    public ParkingSpotsController(IParkingSpotService parkingSpotService)
    {
        _parkingSpotService = parkingSpotService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ParkingSpotDto>>>> GetAll()
    {
        var result = await _parkingSpotService.GetAllAsync();
        return Ok(ApiResponse<List<ParkingSpotDto>>.Ok(result));
    }

    [HttpGet("available")]
    public async Task<ActionResult<ApiResponse<List<ParkingSpotDto>>>> GetAvailable(
        [FromQuery] VehicleType? vehicleType)
    {
        var result = await _parkingSpotService.GetAvailableAsync(vehicleType);
        return Ok(ApiResponse<List<ParkingSpotDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ParkingSpotDto>>> GetById(int id)
    {
        var result = await _parkingSpotService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<ParkingSpotDto>.Fail("Không tìm thấy vị trí đỗ xe."));
        return Ok(ApiResponse<ParkingSpotDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ParkingSpotDto>>> Create([FromBody] CreateParkingSpotRequest request)
    {
        try
        {
            var result = await _parkingSpotService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                ApiResponse<ParkingSpotDto>.Ok(result, "Thêm vị trí đỗ xe thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ParkingSpotDto>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(int id, [FromQuery] ParkingSpotStatus status)
    {
        var success = await _parkingSpotService.UpdateStatusAsync(id, status);
        if (!success)
            return NotFound(ApiResponse.Fail("Không tìm thấy vị trí đỗ xe."));
        return Ok(ApiResponse.Ok("Cập nhật trạng thái thành công."));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        var success = await _parkingSpotService.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse.Fail("Không tìm thấy vị trí đỗ xe."));
        return Ok(ApiResponse.Ok("Xóa vị trí đỗ xe thành công."));
    }
}
