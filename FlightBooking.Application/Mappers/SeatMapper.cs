using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Mappers;

public static class SeatMapper
{
    public static SeatDto ToSeatDto(Seat seat)
    {
        return new SeatDto
        {
            Id = seat.Id,
            FlightId = seat.FlightId,
            SeatNumber = seat.SeatNumber,
            Status = seat.Status.ToString(),
            Price = seat.Price
        };
    }

    public static Seat ToSeat(SeatRequestDto requestDto)
    {
        return new Seat
        {
            SeatNumber = requestDto.SeatNumber,
            Price = requestDto.Price
        };
    }
}