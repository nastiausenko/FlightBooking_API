using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(int userId, BookingRequestDto booking);
    Task<Booking> CancelBookingAsync(int bookingId, int userId, bool isAdmin);
    Task<List<Booking>> CancelUserBookingsByAdminAsync(int userId);
    Task<List<Booking>> GetUserBookingsAsync(int userId);
}