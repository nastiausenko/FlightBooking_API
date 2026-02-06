using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Domain;

namespace FlightBooking.Application.Interfaces;

public interface ISeatService
{
    Task<Seat> AddSeatToFlightAsync(int flightId, Seat seat);
    Task<Seat?> UpdateSeatAsync(int id, SeatRequestDto requestDto);
    Task DeleteSeatAsync(int id);
    Task<Seat?> GetSeatByIdAsync(int id);
    Task<List<Seat>> GetAllByFlightIdAsync(int flightId);
}