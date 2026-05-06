using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGarage.Core.DTOs;
using SmartGarage.Core.Interfaces;

namespace SmartGarage.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetMyNotifications()
    {
        var userId = GetUserId();
        var result = await _notificationService.GetByUserIdAsync(userId);
        return Ok(ApiResponse<List<NotificationDto>>.Ok(result));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        var userId = GetUserId();
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(ApiResponse<int>.Ok(count));
    }

    [HttpPatch("{id}/read")]
    public async Task<ActionResult<ApiResponse>> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok(ApiResponse.Ok("Đã đánh dấu đã đọc."));
    }

    [HttpPatch("read-all")]
    public async Task<ActionResult<ApiResponse>> MarkAllAsRead()
    {
        var userId = GetUserId();
        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(ApiResponse.Ok("Đã đánh dấu tất cả đã đọc."));
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
