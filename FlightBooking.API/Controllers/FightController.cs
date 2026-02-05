using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightController : ControllerBase
{
    private readonly IFlightService _flightService;
    
    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FlightDto>>> GetAll()
    {
        var flights = await _flightService.GetAllFlightsAsync();
        var dto = flights.Select(FlightMapper.ToFlightDto);
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FlightDetailsDto>> GetById(int id)
    {
        var flight = await _flightService.GetFlightByIdAsync(id);
        
        if (flight == null)
        {
            return NotFound();
        }
        
        var dto = FlightMapper.ToFlightDetailsDto(flight);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<FlightDto>> CreateFlight([FromBody] CreateFlightRequestDto requestDto)
    {
        var flight = FlightMapper.ToFlight(requestDto);
        await _flightService.AddFlightAsync(flight);
        
        var dto = FlightMapper.ToFlightDetailsDto(flight);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFlight(int id, [FromBody] UpdateFlightRequestDto updateDto)
    {
        var updatedFlight = await _flightService.UpdateFlightAsync(id, updateDto);
        if (updatedFlight == null)
        {
            return NotFound();
        }
        
        return Ok(FlightMapper.ToFlightDto(updatedFlight));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFlight(int id)
    {
        await _flightService.DeleteFlightAsync(id);
        return NoContent();
    }
}