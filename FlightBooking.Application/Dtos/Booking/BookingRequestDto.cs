using System.ComponentModel.DataAnnotations;
using FlightBooking.Application.Dtos.BookingSeat;

namespace FlightBooking.Application.Dtos.Booking;

public class BookingRequestDto
{
    [Required(ErrorMessage = "BookingSeats is required")]
    [MinLength(1, ErrorMessage = "At least one seat must be provided")]
    public List<BookingSeatRequestDto> BookingSeats { get; set; } = new();
}