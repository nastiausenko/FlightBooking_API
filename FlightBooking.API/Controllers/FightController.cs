using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/flights")]
[Authorize]
public class FlightController(IFlightService flightService) : ControllerBase
{
    /// <summary>
    /// Gets all flights or filtered flights based on query parameters.
    /// </summary>
    /// <param name="queryDto">Optional filter criteria (From/To).</param>
    /// <returns>List of flights as FlightDto.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FlightDto>>> GetAll([FromQuery] FlightQueryDto? queryDto)
    {
        IEnumerable<Flight> flights;
        
        if (queryDto == null)
        {
            flights = await flightService.GetAllFlightsAsync();
        }
        else
        {
            flights = await flightService.GetFlightsAsync(queryDto);
        }
        
        return Ok(flights.Select(FlightMapper.ToFlightDto));
    }

    /// <summary>
    /// Gets flight details by its ID.
    /// </summary>
    /// <param name="flightId">ID of the flight.</param>
    /// <returns>Flight details as FlightDetailsDto.</returns>
    [HttpGet("{flightId:int}")]
    public async Task<ActionResult<FlightDetailsDto>> GetById(int flightId)
    {
        var flight = await flightService.GetFlightByIdAsync(flightId);

        var dto = FlightMapper.ToFlightDetailsDto(flight);
        return Ok(dto);
    }

    /// <summary>
    /// Creates a new flight (admin only).
    /// </summary>
    /// <param name="requestDto">Data for creating a flight.</param>
    /// <returns>The created flight as FlightDetailsDto.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FlightDto>> CreateFlight([FromBody] CreateFlightRequestDto requestDto)
    {
        var flight = FlightMapper.ToFlight(requestDto);
        var createdFlight = await flightService.AddFlightAsync(flight);
        var dto = FlightMapper.ToFlightDetailsDto(createdFlight);
        
        return CreatedAtAction(nameof(GetById), new { flightId = dto.Id }, dto);
    }

    /// <summary>
    /// Updates an existing flight (admin only).
    /// </summary>
    /// <param name="flightId">ID of the flight to update.</param>
    /// <param name="updateDto">Updated flight data.</param>
    /// <returns>The updated flight as FlightDto.</returns>
    [HttpPut("{flightId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFlight(int flightId, [FromBody] UpdateFlightRequestDto updateDto)
    {
        var updatedFlight = await flightService.UpdateFlightAsync(flightId, updateDto);

        return Ok(FlightMapper.ToFlightDto(updatedFlight));
    }

    /// <summary>
    /// Deletes a flight (admin only).
    /// </summary>
    /// <param name="flightId">ID of the flight to delete.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{flightId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFlight(int flightId)
    {
        await flightService.DeleteFlightAsync(flightId);
        return NoContent();
    }
}