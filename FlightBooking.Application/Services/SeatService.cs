using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Application.Exceptions.Flight;
using FlightBooking.Application.Exceptions.Seat;
using FlightBooking.Application.Interfaces;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

/// <summary>
/// Service responsible for managing seats business logic.
/// </summary>
/// <param name="seatRepository"></param>
/// <param name="flightRepository"></param>
public class SeatService(ISeatRepository seatRepository, IFlightRepository flightRepository) : ISeatService
{
    /// <summary>
    /// Adds a new seat to an existing flight.
    /// </summary>
    /// <param name="flightId">Flight ID.</param>
    /// <param name="seat">Seat entity to add.</param>
    /// <returns>Added seat entity.</returns>
    /// <exception cref="FlightNotFoundException">Thrown when the fight does not exist.</exception>
    /// <exception cref="SeatAlreadyExistsException">
    /// Thrown when a seat with the same number already exists for the flight.
    /// </exception>
    public async Task<Seat> AddSeatToFlightAsync(int flightId, Seat seat)
    {
        var flightExists = await flightRepository.ExistsByIdAsync(flightId);
        if (!flightExists)
        {
            throw new FlightNotFoundException(flightId);
        }
        
        var exists = await seatRepository.ExistsByFlightIdAndNumberAsync(flightId, seat.SeatNumber);
        if (exists)
        {
            throw new SeatAlreadyExistsException(seat.SeatNumber, flightId);
        }
        
        seat.FlightId = flightId;
        
        await seatRepository.AddAsync(seat);
        return seat;
    }

    /// <summary>
    /// Updates an existing seat.
    /// </summary>
    /// <param name="id">Seat ID.</param>
    /// <param name="requestDto">Updated seat data.</param>
    /// <returns>Updated seat.</returns>
    /// <exception cref="SeatNotFoundException">Thrown when the seat does not exist.</exception>
    /// <exception cref="SeatAlreadyExistsException">
    /// Thrown when a seat with the same number already exists for the flight.
    /// </exception>
    public async Task<Seat> UpdateSeatAsync(int id, SeatRequestDto requestDto)
    {
        var seat = await seatRepository.GetByIdAsync(id) ?? throw new SeatNotFoundException(id);
        
        var allSeats = await seatRepository.GetByFlightIdAsync(seat.FlightId);

        if (allSeats.Any(s => s.SeatNumber == requestDto.SeatNumber && s.Id != id))
        {
            throw new SeatAlreadyExistsException(requestDto.SeatNumber, seat.FlightId);
        }
      
        seat.Price = requestDto.Price;
        seat.SeatNumber = requestDto.SeatNumber;
        
        await seatRepository.UpdateAsync(seat);
        return seat;
    }
    
    /// <summary>
    /// Deletes a seat and updates total prices of related bookings.
    /// </summary>
    /// <param name="id">Seat ID.</param>
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

    /// <summary>
    /// Returns a seat by its ID.
    /// </summary>
    /// <param name="id">Seat ID.</param>
    /// <exception cref="SeatNotFoundException">Thrown when the seat does not exist.</exception>
    public async Task<Seat> GetSeatByIdAsync(int id) => 
        await seatRepository.GetByIdAsync(id) ?? throw new SeatNotFoundException(id);
    
    /// <summary>
    /// Returns all seats for the specified flight.
    /// </summary>
    /// <param name="flightId">Flight ID.</param>
    /// <exception cref="FlightNotFoundException">Thrown when the fight does not exist.</exception>
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