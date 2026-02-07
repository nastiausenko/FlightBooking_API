using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Repositories;

public class SeatRepository(FlightBookingDbContext dbContext) : ISeatRepository
{ 
    public async Task<Seat?> GetByIdAsync(int seatId)
    {
        return await dbContext.Seats
            .Include(s => s.BookingSeats)
            .ThenInclude(bs => bs.Booking)
            .FirstOrDefaultAsync(s => s.Id == seatId);
    }

    public async Task<List<Seat>> GetByIdsAsync(List<int> seatIds)
    {
        return await dbContext.Seats
            .Where(s => seatIds.Contains(s.Id))
            .ToListAsync();
    }

    public async Task<List<Seat>> GetByFlightIdAsync(int flightId)
    {
        return await dbContext.Seats
            .Where(s => s.FlightId == flightId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Seat seat)
    {
        dbContext.Seats.Update(seat);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Seat seat)
    {
        dbContext.Seats.Remove(seat);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddAsync(Seat seat)
    {
        await dbContext.Seats.AddAsync(seat);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsByFlightIdAndNumberAsync(int flightId, string seatNumber)
    {
        return await  dbContext.Seats.AnyAsync(s => s.FlightId == flightId && s.SeatNumber == seatNumber);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}