using FlightBooking.Application.Dtos.Seat;

namespace FlightBooking.Application.Dtos.Flight;

public class FlightDetailsDto : FlightDto
{
    public List<SeatDto>? Seats { get; set; }
}