using System.Security.Claims;
using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingRequestDto requestDto)
    {
        var userId = GetUserId();

        var booking = await bookingService.CreateBookingAsync(userId, requestDto);
        return Ok(BookingMapper.ToBookingDto(booking));
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetUserBookingsAsync()
    {
        var userId = GetUserId();

        var bookings = await bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings.Select(BookingMapper.ToBookingDto));
    }

    [HttpPut("{bookingId:int}/cancel")]
    public async Task<ActionResult<BookingDto>> CancelBooking(int bookingId)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        var booking = await bookingService.CancelBookingAsync(bookingId, userId,  isAdmin);

        return Ok(BookingMapper.ToBookingDto(booking));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("users/{userId:int}/cancel")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> CancelUserBookingsByAdmin(int userId)
    {
        var bookings = await bookingService.CancelUserBookingsByAdminAsync(userId);
        return Ok(bookings.Select(BookingMapper.ToBookingDto));
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException();
        }
        return int.Parse(userIdClaim);
    }
}