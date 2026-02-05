using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/bookings")]
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
}