using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Application.Exceptions;
using FlightBooking.Application.Interfaces;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _seatRepository;
    private readonly IFlightRepository _flightRepository;

    public SeatService(ISeatRepository seatRepository, IFlightRepository flightRepository)
    {
        _seatRepository = seatRepository;
        _flightRepository = flightRepository;
    }
    
    public async Task<Seat> AddSeatToFlightAsync(int flightId, Seat seat)
    {
        var flight = await _flightRepository.GetByIdAsync(flightId);
        if (flight == null)
        {
            throw new FlightNotFoundException(flightId);
        }
        
        seat.FlightId = flightId;
        
        await _seatRepository.AddAsync(seat);
        
        return seat;
    }

    public async Task<Seat> UpdateSeatAsync(int id, SeatRequestDto requestDto)
    {
        var seat = await _seatRepository.GetByIdAsync(id);
        if (seat == null)
        {
            throw new SeatNotFoundException(id);
        }
        
        seat.Price = requestDto.Price;
        seat.SeatNumber = requestDto.SeatNumber;
        
        await _seatRepository.UpdateAsync(seat);
        return seat;
    }

    public async Task DeleteSeatAsync(int id)
    {
        var seat = await _seatRepository.GetByIdAsync(id);

        if (seat == null)
        {
            return;
        }

        foreach (var bookingSeat in seat.BookingSeats)
        {
            var booking = bookingSeat.Booking;
            booking.TotalPrice -= bookingSeat.Price;
        }

        await _seatRepository.DeleteAsync(seat);
    }

    public async Task<Seat> GetSeatByIdAsync(int id) => 
        await _seatRepository.GetByIdAsync(id) 
        ?? throw new SeatNotFoundException(id);
    
    public async Task<List<Seat>> GetAllByFlightIdAsync(int flightId)
    {
        var flight = await _flightRepository.GetByIdAsync(flightId);
        if (flight == null)
        {
            throw new FlightNotFoundException(flightId);
        }
        
        return await _seatRepository.GetByFlightIdAsync(flightId);
    }
}