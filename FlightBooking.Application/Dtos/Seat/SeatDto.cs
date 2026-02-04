using FlightBooking.Domain;

namespace FlightBooking.Application.Dtos.Seat;

public class SeatDto
{
    public int Id { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal? Price { get; set; } = null;
    public bool? IsCancelled { get; set; } = null;
}