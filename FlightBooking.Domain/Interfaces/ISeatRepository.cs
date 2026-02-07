using FlightBooking.Domain.Models;

namespace FlightBooking.Domain.Interfaces;

public interface ISeatRepository
{
    Task<Seat?> GetByIdAsync(int seatId);
    Task<List<Seat>> GetByIdsAsync(List<int> seatIds);
    Task<List<Seat>> GetByFlightIdAsync(int flightId);
    Task UpdateAsync(Seat seat);
    Task DeleteAsync(Seat seat);
    Task AddAsync(Seat seat);
    Task<bool> ExistsByFlightIdAndNumberAsync(int flightId, string seatNumber);
}