using FlightBooking.Domain.Models;

namespace FlightBooking.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int bookingId);
    Task<List<Booking>> GetByUserIdAsync(int userId);
    Task<List<Booking>> GetActiveByUserIdAsync(int userId);
    Task AddAsync(Booking booking);
    Task SaveChangesAsync();
}