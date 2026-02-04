using FlightBooking.Appication.Dtos.Flight;
using FlightBooking.Appication.Dtos.Seat;
using FlightBooking.Domain;

namespace FlightBooking.Appication.Mappers;

public static class FlightMapper
{
    public static FlightDto ToFlightDto(Flight flight)
    {
        return new FlightDto
        {
            Id = flight.Id,
            From = flight.From,
            To = flight.To,
            FlightNumber = flight.FlightNumber,
            Arrival = flight.Arrival,
            Departure = flight.Departure
        };
    }
    
    public static FlightDetailsDto ToFlightDetailsDto(Flight flight)
    {
        return new FlightDetailsDto
        {
            Id = flight.Id,
            From = flight.From,
            To = flight.To,
            FlightNumber = flight.FlightNumber,
            Departure = flight.Departure,
            Arrival = flight.Arrival,
            Seats = flight.Seats.Select(s =>
            {
                var bookingSeat = s.BookingSeats.FirstOrDefault(bs => !bs.IsCancelled);
                return new SeatDto
                {
                    Id = s.Id,
                    SeatNumber = s.SeatNumber,
                    Status = s.Status.ToString(),
                    Price = bookingSeat?.Price,
                    IsCancelled = bookingSeat?.IsCancelled
                };
            }).ToList()
        };
    }
}