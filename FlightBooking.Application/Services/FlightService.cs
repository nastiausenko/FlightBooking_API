using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Exceptions.Flight;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

/// <summary>
/// Service responsible for flight management logic
/// </summary>
/// <param name="flightRepository"></param>
public class FlightService(IFlightRepository flightRepository) : IFlightService
{
    /// <summary>
    /// Returns all available flights.
    /// </summary>
    /// <returns>List of flights.</returns>
    public async Task<List<Flight>> GetAllFlightsAsync() => await flightRepository.GetAllAsync();
    
    /// <summary>
    /// Returns a flight by its ID.
    /// </summary>
    /// <param name="id">Flight ID.</param>
    /// <exception cref="FlightNotFoundException">Thrown when a flight does not exist.</exception>
    public async Task<Flight> GetFlightByIdAsync(int id) =>
        await flightRepository.GetByIdAsync(id) ?? throw new FlightNotFoundException(id);

    /// <summary>
    /// Creates a new flight.
    /// </summary>
    /// <param name="flight">Flight entity to create.</param>
    /// <returns>Created flight.</returns>
    /// <exception cref="FlightAlreadyExistsException">
    /// Thrown when a flight with the same flight number already exists.
    /// </exception>
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

    /// <summary>
    /// Updates an existing flight by its ID.
    /// </summary>
    /// <param name="id">Flight ID.</param>
    /// <param name="dto">Updated flight data.</param>
    /// <returns>Updated flight.</returns>
    /// <exception cref="FlightNotFoundException">Thrown when the flight does not exist.</exception>
    /// <exception cref="FlightAlreadyExistsException">Thrown when a flight with the same number already exists.</exception>
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
            var allFlights = await flightRepository.GetAllAsync();
            if (allFlights.Any(f => f.FlightNumber == dto.FlightNumber && f.Id != id))
            {
                throw new FlightAlreadyExistsException(dto.FlightNumber);
            }
        }
        
        var model = FlightMapper.ToFlight(dto);
        model.Id = id;
        
        await flightRepository.UpdateAsync(id, model);
        return model;
    }

    /// <summary>
    /// Deletes a flight by its ID.
    /// </summary>
    /// <param name="id">Flight ID.</param>
    public async Task DeleteFlightAsync(int id) => await flightRepository.DeleteAsync(id);
    
    /// <summary>
    /// Returns flights filtered by origin and destination.
    /// </summary>
    /// <param name="queryDto">Flight search filters.</param>
    public async Task<List<Flight>> GetFlightsAsync(FlightQueryDto queryDto)
    {
        var flights = await flightRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(queryDto.From))
        {
            flights = flights
                .Where(f => f.From.Contains(queryDto.From, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        
        if (!string.IsNullOrWhiteSpace(queryDto.To))
        {
            flights = flights
                .Where(f => f.To.Contains(queryDto.To, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        
        return flights;
    }
}