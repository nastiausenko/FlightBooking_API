using FlightBooking.Application.Dtos.BookingSeat;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Mappers;

public static class BookingSeatMapper
{
    public static BookingSeatDto ToBookingSeatDto(BookingSeat bookingSeat)
    {
        return new BookingSeatDto
        {
            Id = bookingSeat.Id,
            SeatId = bookingSeat.SeatId,
            BookingId = bookingSeat.BookingId,
            SeatNumber = bookingSeat.Seat.SeatNumber,
            IsCancelled = bookingSeat.IsCancelled,
            Price = bookingSeat.Price
        };
    }

    public static BookingSeat ToBookingSeat(BookingSeatRequestDto dto)
    {
        return new BookingSeat
        {
            SeatId = dto.SeatId
        };
    }
}