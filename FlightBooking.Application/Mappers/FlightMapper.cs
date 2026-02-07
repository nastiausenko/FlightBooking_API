using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Domain.Models;

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
            Seats = flight.Seats.Select(SeatMapper.ToSeatDto).ToList()
        };
    }

    public static Flight ToFlight(CreateFlightRequestDto requestDto)
    {
        var flight = new Flight
        {
            From = requestDto.From,
            To = requestDto.To,
            FlightNumber = requestDto.FlightNumber,
            Departure = requestDto.Departure,
            Arrival = requestDto.Arrival
        };

        if (requestDto.Seats != null && requestDto.Seats.Any())
        {
            flight.Seats = requestDto.Seats.Select(SeatMapper.ToSeat).ToList();
        }

        return flight;
    }

    public static Flight ToFlight(UpdateFlightRequestDto requestDto)
    {
        var flight = new Flight
        {
            From = requestDto.From,
            To = requestDto.To,
            FlightNumber = requestDto.FlightNumber,
            Departure = requestDto.Departure,
            Arrival = requestDto.Arrival
        };
        
        return flight;
    }
}