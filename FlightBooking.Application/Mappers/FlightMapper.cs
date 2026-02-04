using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Domain;

namespace FlightBooking.Application.Mappers;

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
            Seats = flight.Seats.Select(s => new SeatDto
            {
                Id = s.Id,
                SeatNumber = s.SeatNumber,
                Status = s.Status.ToString(),
                Price = s.Price,          
                IsCancelled = false    
            }).ToList()
        };
    }

    public static Flight ToFlight(FlightRequestDto requestDto)
    {
        var flight = new Flight
        {
            From = requestDto.From,
            To = requestDto.To,
            FlightNumber = requestDto.FlightNumber,
            Departure = requestDto.Departure,
            Arrival = requestDto.Arrival
        };

        foreach (var seatDto in requestDto.Seats)
        {
            flight.Seats.Add(new Seat
            {
                SeatNumber = seatDto.SeatNumber,
                Price = seatDto.Price,
                Status = SeatStatus.Available
            });
        }

        return flight;
    }
}