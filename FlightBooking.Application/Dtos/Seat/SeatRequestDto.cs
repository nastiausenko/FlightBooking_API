namespace FlightBooking.Application.Dtos.Seat;

public class SeatRequestDto
{
    public string SeatNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
}