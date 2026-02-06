using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Mappers;

public static class BookingMapper
{
    public static BookingDto ToBookingDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            BookingDate = booking.BookingDate,
            IsCancelled = booking.IsCancelled,
            TotalPrice = booking.TotalPrice,
            BookingSeats = booking.BookingSeats.Select(BookingSeatMapper.ToBookingSeatDto).ToList()
        };
    }
}