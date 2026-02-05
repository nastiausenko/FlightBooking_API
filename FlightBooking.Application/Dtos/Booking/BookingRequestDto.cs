using FlightBooking.Application.Dtos.BookingSeat;

namespace FlightBooking.Application.Dtos.Booking;

public class BookingRequestDto
{
    public List<BookingSeatRequestDto> BookingSeats { get; set; } = new();
}