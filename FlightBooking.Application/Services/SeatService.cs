using System.Reflection.Metadata.Ecma335;
using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Application.Exceptions;
using FlightBooking.Application.Interfaces;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

public class SeatService(ISeatRepository seatRepository, IFlightRepository flightRepository) : ISeatService
{
    public async Task<Seat> AddSeatToFlightAsync(int flightId, Seat seat)
    {
        var exists = await flightRepository.ExistsByIdAsync(flightId);
        if (!exists)
        {
            throw new FlightNotFoundException(flightId);
        }
        
        seat.FlightId = flightId;
        
        await seatRepository.AddAsync(seat);
        return seat;
    }

    public async Task<Seat> UpdateSeatAsync(int id, SeatRequestDto requestDto)
    {
        var seat = await seatRepository.GetByIdAsync(id) ?? throw new SeatNotFoundException(id);
      
        seat.Price = requestDto.Price;
        seat.SeatNumber = requestDto.SeatNumber;
        
        await seatRepository.UpdateAsync(seat);
        return seat;
    }

    public async Task DeleteSeatAsync(int id)
    {
        var seat = await seatRepository.GetByIdAsync(id);
        if (seat == null)
        {
            return;
        }

        foreach (var bookingSeat in seat.BookingSeats)
        {
            var booking = bookingSeat.Booking;
            booking.TotalPrice -= bookingSeat.Price;
        }

        await seatRepository.DeleteAsync(seat);
    }

    public async Task<Seat> GetSeatByIdAsync(int id) => 
        await seatRepository.GetByIdAsync(id) ?? throw new SeatNotFoundException(id);
    
    public async Task<List<Seat>> GetAllByFlightIdAsync(int flightId)
    {
        var exists = await flightRepository.ExistsByIdAsync(flightId);
        if (!exists)
        {
            throw new FlightNotFoundException(flightId);
        }
        
        return await seatRepository.GetByFlightIdAsync(flightId);
    }
}