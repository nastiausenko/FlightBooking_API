using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;
    
    public FlightService(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }
    
    public async Task<List<Flight>> GetAllFlightsAsync()
    {
        return await _flightRepository.GetAllAsync();
    }

    public async Task<Flight?> GetFlightByIdAsync(int id)
    {
        return await _flightRepository.GetByIdAsync(id);
    }

    public async Task<Flight> AddFlightAsync(Flight flight)
    {
        await _flightRepository.AddAsync(flight);
        return flight;
    }

    public async Task<Flight?> UpdateFlightAsync(int id, UpdateFlightRequestDto dto)
    {
        var flight = await _flightRepository.GetByIdAsync(id);
        if (flight == null)
        {
            return null;
        }
        
        var model = FlightMapper.ToFlight(dto);
        
        await _flightRepository.UpdateAsync(id, model);
        return await _flightRepository.GetByIdAsync(id);
    }

    public async Task DeleteFlightAsync(int id)
    {
        await _flightRepository.DeleteAsync(id);
    }
}