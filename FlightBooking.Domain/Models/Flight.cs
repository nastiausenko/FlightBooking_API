namespace FlightBooking.Domain.Models;

public class Flight
{
    public int Id { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public required string FlightNumber { get; set; }
    public DateTime Departure { get; set; }
    public DateTime Arrival { get; set; }
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}