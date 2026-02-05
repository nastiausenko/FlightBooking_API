using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Domain;

namespace FlightBooking.Application.Interfaces;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(int userId, BookingRequestDto booking); //TODO temporary, instead of JWT
    Task<Booking> CancelBookingAsync(int bookingId); //for user
    Task<List<Booking>> CancelBookingByAdminAsync(int? bookingId, int? userId); //for admin
    Task<List<Booking>> GetUserBookingsAsync(int userId); //TODO temporary, instead of JWT
    Task<Booking?> GetBookingByIdAsync(int bookingId);
}