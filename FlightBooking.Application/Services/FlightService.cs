using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Exceptions.Flight;
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
        var exists = await flightRepository.ExistsByNumberAsync(flight.FlightNumber);
        if (exists)
        {
            throw new FlightAlreadyExistsException(flight.FlightNumber);
        }
        await flightRepository.AddAsync(flight);
        return flight;
    }

    public async Task<Flight> UpdateFlightAsync(int id, UpdateFlightRequestDto dto)
    {
        var existsById = await flightRepository.ExistsByIdAsync(id);
        if (!existsById)
        {
            throw new FlightNotFoundException(id);
        }
        
        var exists = await flightRepository.ExistsByNumberAsync(dto.FlightNumber);
        if (exists)
        {
            throw new FlightAlreadyExistsException(dto.FlightNumber);
        }
        
        var model = FlightMapper.ToFlight(dto);
        model.Id = id;
        
        await flightRepository.UpdateAsync(id, model);
        return model;
    }

    public async Task DeleteFlightAsync(int id) => await flightRepository.DeleteAsync(id);
}