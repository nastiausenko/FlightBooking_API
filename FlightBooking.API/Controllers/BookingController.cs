using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromQuery] int userId, [FromBody] BookingRequestDto requestDto)
    {
        var booking = await _bookingService.CreateBookingAsync(userId, requestDto);
        return Ok(BookingMapper.ToBookingDto(booking));
    }

    [HttpGet("users/{userId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings.Select(BookingMapper.ToBookingDto));
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<BookingDto>> CancelBooking([FromQuery] int userId, [FromRoute] int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        
        if (booking == null)
        {
            return NotFound();
        }

        if (booking.UserId != userId)
        {
            return BadRequest("You cannot cancel someone else's booking");
        }

        booking = await _bookingService.CancelBookingAsync(id);

        return Ok(BookingMapper.ToBookingDto(booking));
    }
    
    [HttpPut("admin/cancel")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> CancelBookingByAdmin(
        [FromQuery] int? bookingId,
        [FromQuery] int? userId)
    {
        var bookingsToCancel = await _bookingService.CancelBookingByAdminAsync(bookingId, userId);
        return Ok(bookingsToCancel.Select(BookingMapper.ToBookingDto));
    }
}