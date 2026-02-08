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
    /// <summary>
    /// Creates a new booking for authenticated user.
    /// </summary>
    /// <param name="requestDto">Booking request with selected seats.</param>
    /// <returns>The created booking as BookingDto.</returns>
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingRequestDto requestDto)
    {
        var userId = GetUserId();

        var booking = await bookingService.CreateBookingAsync(userId, requestDto);
        return Ok(BookingMapper.ToBookingDto(booking));
    }

    /// <summary>
    /// Return all bookings of the authenticated user.
    /// </summary>
    /// <returns>List of BookingDto.</returns>
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetUserBookingsAsync()
    {
        var userId = GetUserId();

        var bookings = await bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings.Select(BookingMapper.ToBookingDto));
    }

    /// <summary>
    /// Cancels a booking of the authenticated user or admin.
    /// </summary>
    /// <param name="bookingId">ID of the booking to cancel.</param>
    /// <returns>The canceled booking as BookingDto.</returns>
    [HttpPut("{bookingId:int}/cancel")]
    public async Task<ActionResult<BookingDto>> CancelBooking(int bookingId)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        var booking = await bookingService.CancelBookingAsync(bookingId, userId,  isAdmin);

        return Ok(BookingMapper.ToBookingDto(booking));
    }
    
    /// <summary>
    /// Cancels all active bookings of a specified user (admin only).
    /// </summary>
    /// <param name="userId">ID of the user whose bookings to cancel.</param>
    /// <returns>List of canceled bookings as BookingDto.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("users/{userId:int}/cancel")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> CancelUserBookingsByAdmin(int userId)
    {
        var bookings = await bookingService.CancelUserBookingsByAdminAsync(userId);
        return Ok(bookings.Select(BookingMapper.ToBookingDto));
    }

    // Gets the ID of the currently authenticated user from claims.
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