using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Interfaces;

public interface IFlightService
{
    Task<List<Flight>> GetAllFlightsAsync();
    Task<Flight> GetFlightByIdAsync(int id);
    Task<Flight> AddFlightAsync(Flight flight);
    Task<Flight> UpdateFlightAsync(int id, UpdateFlightRequestDto dto);
    Task DeleteFlightAsync(int id);
    Task<List<Flight>> GetFlightsAsync(FlightQueryDto queryDto);
}