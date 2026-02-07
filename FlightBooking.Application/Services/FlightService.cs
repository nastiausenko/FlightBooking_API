using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Exceptions;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

public class FlightService(IFlightRepository flightRepository) : IFlightService
{
    public async Task<List<Flight>> GetAllFlightsAsync() => await flightRepository.GetAllAsync();
    
    public async Task<Flight> GetFlightByIdAsync(int id) =>
        await flightRepository.GetByIdAsync(id) ?? throw new FlightNotFoundException(id);

    public async Task<Flight> AddFlightAsync(Flight flight)
    {
        await flightRepository.AddAsync(flight);
        return flight;
    }

    public async Task<Flight> UpdateFlightAsync(int id, UpdateFlightRequestDto dto)
    {
        var exists = await flightRepository.ExistsByIdAsync(id);
        if (!exists)
        {
            throw new FlightNotFoundException(id);
        }
        
        var model = FlightMapper.ToFlight(dto);
        
        await flightRepository.UpdateAsync(id, model);
        return model;
    }

    public async Task DeleteFlightAsync(int id) => await flightRepository.DeleteAsync(id);
}