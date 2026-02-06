namespace FlightBooking.Application.Dtos.Seat;

public class SeatDto
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}