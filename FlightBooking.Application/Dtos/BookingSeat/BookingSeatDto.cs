namespace FlightBooking.Application.Dtos.BookingSeat;

public class BookingSeatDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int SeatId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public bool IsCancelled { get; set; } 
    public decimal Price { get; set; }
}