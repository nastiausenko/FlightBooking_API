using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly FlightBookingDbContext _dbContext;

    public SeatRepository(FlightBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Seat?> GetByIdAsync(int seatId)
    {
        return await _dbContext.Seats
            .Include(s => s.BookingSeats)
            .ThenInclude(bs => bs.Booking)
            .FirstOrDefaultAsync(s => s.Id == seatId);
    }

    public async Task<List<Seat>> GetByIdsAsync(List<int> seatIds)
    {
        return await _dbContext.Seats
            .Where(s => seatIds.Contains(s.Id))
            .ToListAsync();
    }

    public async Task<List<Seat>> GetByFlightIdAsync(int flightId)
    {
        return await _dbContext.Seats
            .Where(s => s.FlightId == flightId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Seat seat)
    {
        _dbContext.Seats.Update(seat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Seat seat)
    {
        _dbContext.Seats.Remove(seat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddAsync(Seat seat)
    {
        await _dbContext.Seats.AddAsync(seat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}