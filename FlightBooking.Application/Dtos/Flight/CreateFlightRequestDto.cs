using FlightBooking.Application.Dtos.Seat;

namespace FlightBooking.Application.Dtos.Flight;

public class CreateFlightRequestDto
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    
    public DateTime Departure { get; set; }
    public DateTime Arrival { get; set; }
    
    public List<SeatRequestDto>? Seats { get; set; }
}