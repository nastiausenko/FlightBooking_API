using FlightBooking.Application.Dtos.BookingSeat;

namespace FlightBooking.Application.Dtos.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.Now;
    public bool IsCancelled { get; set; }
    public List<BookingSeatDto>? BookingSeats { get; set; } 
    public decimal TotalPrice { get; set; } 
}