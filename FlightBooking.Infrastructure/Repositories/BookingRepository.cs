using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly FlightBookingDbContext _dbContext;

    public BookingRepository(FlightBookingDbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<Booking?> GetByIdAsync(int bookingId)
    {
        return await _dbContext.Bookings
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }

    public async Task<List<Booking>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetActiveByUserIdAsync(int userId)
    {
        return await _dbContext.Bookings
            .Where(b => b.UserId == userId && !b.IsCancelled)
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .ToListAsync();
    }

    public async Task AddAsync(Booking booking)
    { 
        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}