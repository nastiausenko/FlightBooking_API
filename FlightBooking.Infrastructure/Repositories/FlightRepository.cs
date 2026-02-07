using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Repositories;

public class FlightRepository(FlightBookingDbContext dbContext) : IFlightRepository
{
    public async Task<List<Flight>> GetAllAsync()
    {
        return await dbContext.Flights.ToListAsync();
    }

    public async Task<Flight?> GetByIdAsync(int flightId)
    {
        return await dbContext.Flights
            .Include(f => f.Seats)
            .FirstOrDefaultAsync(f => f.Id == flightId);
    }

    public async Task AddAsync(Flight flight)
    {
        dbContext.Flights.Add(flight);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, Flight model)
    {
        await dbContext.Flights
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
        await dbContext.Flights
            .Where(f => f.Id == flightId)
            .ExecuteDeleteAsync();
    }

    public async Task<bool> ExistsByIdAsync(int flightId)
    {
        return await dbContext.Flights.AnyAsync(f => f.Id == flightId);
    }

    public async Task<bool> ExistsByNumberAsync(string flightNumber)
    {
       return await dbContext.Flights.AnyAsync(f => f.FlightNumber == flightNumber);
    }
}