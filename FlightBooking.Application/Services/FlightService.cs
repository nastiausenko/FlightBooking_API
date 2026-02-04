using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Interfaces;
using FlightBooking.Domain;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Application.Services;

public class FlightService : IFlightService
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

    public async Task<Flight> AddFlightAsync(Flight flight)
    {
        _dbContext.Flights.Add(flight);
        await _dbContext.SaveChangesAsync();
        return flight;
    }

    public async Task<Flight?> UpdateFlightAsync(int id, UpdateFlightRequestDto dto)
    {
        var flight = await _dbContext.Flights.FirstOrDefaultAsync(f => f.Id == id);
        if (flight == null)
        {
            return null;
        }
        
        flight.From = dto.From;
        flight.To = dto.To;
        flight.Departure = dto.Departure;
        flight.Arrival = dto.Arrival;
        flight.FlightNumber = dto.FlightNumber;

        await _dbContext.SaveChangesAsync();
        return flight;
    }

    public async Task DeleteFlightAsync(int id)
    {
        var flight = await _dbContext.Flights.FirstOrDefaultAsync(f => f.Id == id);
        if (flight == null)
            return;

        _dbContext.Flights.Remove(flight);
        await _dbContext.SaveChangesAsync();
    }
}