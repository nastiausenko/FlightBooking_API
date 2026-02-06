using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Application.Interfaces;
using FlightBooking.Domain;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Application.Services;

public class SeatService : ISeatService
{
    private readonly FlightBookingDbContext _dbContext;

    public SeatService(FlightBookingDbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<Seat> AddSeatToFlightAsync(int flightId, Seat seat)
    {
        var flight = await _dbContext.Flights
            .Include(f => f.Seats)
            .FirstOrDefaultAsync(f => f.Id == flightId);
        
        if (flight == null)
        {
            throw new KeyNotFoundException("Flight not found");
        }
        
        seat.FlightId = flightId;
        
        _dbContext.Seats.Add(seat);
        await _dbContext.SaveChangesAsync();
        
        return seat;
    }

    public async Task<Seat?> UpdateSeatAsync(int id, SeatRequestDto requestDto)
    {
        var seat = await _dbContext.Seats.FirstOrDefaultAsync(s => s.Id == id);
        if (seat == null)
        {
            return null;
        }
        
        seat.Price = requestDto.Price;
        seat.SeatNumber = requestDto.SeatNumber;
        
        await _dbContext.SaveChangesAsync();
        return seat;
    }

    public async Task DeleteSeatAsync(int id)
    {
        var seat = await _dbContext.Seats
            .Include(s => s.BookingSeats)
            .ThenInclude(bs => bs.Booking)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (seat == null)
            return;

        foreach (var bookingSeat in seat.BookingSeats)
        {
            var booking = bookingSeat.Booking;
            booking.TotalPrice -= bookingSeat.Price;

            _dbContext.BookingSeats.Remove(bookingSeat);
        }

        _dbContext.Seats.Remove(seat);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Seat?> GetSeatByIdAsync(int id)
    {
        return await _dbContext.Seats
            .Include(s => s.Flight)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Seat>> GetAllByFlightIdAsync(int flightId)
    {
        var flight = await _dbContext.Flights
            .Include(f => f.Seats)
            .FirstOrDefaultAsync(f => f.Id == flightId);
        
        if (flight == null)
        {
            throw new KeyNotFoundException("Flight not found");
        }
        
        return await _dbContext.Seats.Where(s => s.FlightId == flightId).ToListAsync();
    }
}