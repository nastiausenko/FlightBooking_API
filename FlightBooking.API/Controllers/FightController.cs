using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Mappers;
using FlightBooking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase
{
    private readonly FlightService _flightService;
    
    public FlightController(FlightService flightService)
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
    public async Task<ActionResult<FlightDto>> CreateFlight([FromBody] FlightRequestDto requestDto)
    {
        var flight = FlightMapper.ToFlight(requestDto);
        await _flightService.AddFlightAsync(flight);
        
        var dto = FlightMapper.ToFlightDetailsDto(flight);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFlight(int id, [FromBody] UpdateFlightRequest requestDto)
    {
        var flight = await _flightService.GetFlightByIdAsync(id);
        if (flight == null)
        {
            return NotFound();
        }
        
        flight.From = requestDto.From;
        flight.To = requestDto.To;
        flight.Arrival = requestDto.Arrival;
        flight.Departure = requestDto.Departure;
        flight.FlightNumber = requestDto.FlightNumber;
        
        await _flightService.UpdateFlightAsync(flight);
        return Ok(FlightMapper.ToFlightDto(flight));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFlight(int id)
    {
        var flight = await _flightService.GetFlightByIdAsync(id);
        if (flight != null)
        {
            await _flightService.DeleteFlightAsync(flight);
        }
        
        return NoContent();
    }
}