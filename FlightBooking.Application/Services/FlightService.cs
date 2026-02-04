using FlightBooking.Domain;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Application.Services;

public class FlightService
{
    private readonly FlightBookingDbContext _dbContext;
    
    public FlightService(FlightBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Flight>> GetAllFlightsAsync()
    {
        return await _dbContext.Flights.ToListAsync();
    }

    public async Task<Flight?> GetFlightByIdAsync(int id)
    {
        return await _dbContext.Flights
            .Include(f => f.Seats)
            .ThenInclude(s => s.BookingSeats)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
}