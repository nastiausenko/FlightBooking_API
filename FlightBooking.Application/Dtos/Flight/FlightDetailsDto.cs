using FlightBooking.Application.Dtos.Seat;

namespace FlightBooking.Application.Dtos.Flight;

public class FlightDetailsDto : FlightDto
{
    public ICollection<SeatDto> Seats { get; set; } = new List<SeatDto>();
}