using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarage.Core.DTOs;
using SmartGarage.Core.DTOs.Vehicle;
using SmartGarage.Core.Interfaces;

namespace SmartGarage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<ActionResult<ApiResponse<List<VehicleDto>>>> GetAll()
    {
        var result = await _vehicleService.GetAllAsync();
        return Ok(ApiResponse<List<VehicleDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<VehicleDto>>> GetById(int id)
    {
        var result = await _vehicleService.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse<VehicleDto>.Fail("Không tìm thấy phương tiện."));
        return Ok(ApiResponse<VehicleDto>.Ok(result));
    }

    [HttpGet("my-vehicles")]
    public async Task<ActionResult<ApiResponse<List<VehicleDto>>>> GetMyVehicles()
    {
        var userId = GetUserId();
        var result = await _vehicleService.GetByOwnerIdAsync(userId);
        return Ok(ApiResponse<List<VehicleDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<VehicleDto>>> Create([FromBody] CreateVehicleRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _vehicleService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                ApiResponse<VehicleDto>.Ok(result, "Thêm phương tiện thành công."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<VehicleDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<VehicleDto>>> Update(int id, [FromBody] UpdateVehicleRequest request)
    {
        var result = await _vehicleService.UpdateAsync(id, request);
        if (result == null)
            return NotFound(ApiResponse<VehicleDto>.Fail("Không tìm thấy phương tiện."));
        return Ok(ApiResponse<VehicleDto>.Ok(result, "Cập nhật thành công."));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        var success = await _vehicleService.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse.Fail("Không tìm thấy phương tiện."));
        return Ok(ApiResponse.Ok("Xóa phương tiện thành công."));
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
