using FlightBooking.Appication.Dtos.Seat;

namespace FlightBooking.Appication.Dtos.Flight;

public class FlightDetailsDto : FlightDto
{
    public ICollection<SeatDto> Seats { get; set; } = new List<SeatDto>();
}