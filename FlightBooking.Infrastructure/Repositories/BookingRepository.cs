using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Repositories;

public class BookingRepository(FlightBookingDbContext dbContext) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(int bookingId)
    {
        return await dbContext.Bookings
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }

    public async Task<List<Booking>> GetByUserIdAsync(int userId)
    {
        return await dbContext.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .ToListAsync();
    }

    // Returns all user bookings that are not canceled
    public async Task<List<Booking>> GetActiveByUserIdAsync(int userId)
    {
        return await dbContext.Bookings
            .Where(b => b.UserId == userId && !b.IsCancelled)
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .ToListAsync();
    }

    public async Task AddAsync(Booking booking)
    { 
        await dbContext.Bookings.AddAsync(booking);
        await dbContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}