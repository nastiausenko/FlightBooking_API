using FlightBooking.Domain.Models;

namespace FlightBooking.Domain.Interfaces;

public interface IFlightRepository
{
    Task<List<Flight>> GetAllAsync();
    Task<Flight?> GetByIdAsync(int flightId);
    Task AddAsync(Flight flight);
    Task UpdateAsync(int id, Flight model);
    Task DeleteAsync(int flightId);
    Task<bool> ExistsByIdAsync(int flightId);
    Task<bool> ExistsByNumberAsync(string flightNumber);
}