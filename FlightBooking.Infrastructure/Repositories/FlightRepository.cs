using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly FlightBookingDbContext _dbContext;
    
    public FlightRepository(FlightBookingDbContext context)
    {
        _dbContext = context;
    }

    public async Task<List<Flight>> GetAllAsync()
    {
        return await _dbContext.Flights.ToListAsync();
    }

    public async Task<Flight?> GetByIdAsync(int flightId)
    {
        return await _dbContext.Flights
            .Include(f => f.Seats)
            .FirstOrDefaultAsync(f => f.Id == flightId);
    }

    public async Task AddAsync(Flight flight)
    {
        _dbContext.Flights.Add(flight);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Flight model)
    {
        await _dbContext.Flights
            .Where(f => f.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(f => f.From, model.From)
                .SetProperty(f => f.To, model.To)
                .SetProperty(f => f.Departure, model.Departure)
                .SetProperty(f => f.Arrival, model.Arrival)
                .SetProperty(f => f.FlightNumber, model.FlightNumber));
    }

    public async Task DeleteAsync(int flightId)
    {
        await _dbContext.Flights
            .Where(f => f.Id == flightId)
            .ExecuteDeleteAsync();
    }
}