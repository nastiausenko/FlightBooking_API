using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/flights")]
[Authorize]
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

    [HttpGet("{flightId:int}")]
    public async Task<ActionResult<FlightDetailsDto>> GetById(int flightId)
    {
        var flight = await _flightService.GetFlightByIdAsync(flightId);
        
        if (flight == null)
        {
            return NotFound();
        }
        
        var dto = FlightMapper.ToFlightDetailsDto(flight);
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FlightDto>> CreateFlight([FromBody] CreateFlightRequestDto requestDto)
    {
        var flight = FlightMapper.ToFlight(requestDto);
        await _flightService.AddFlightAsync(flight);

        var dto = FlightMapper.ToFlightDetailsDto(flight);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{flightId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFlight(int flightId, [FromBody] UpdateFlightRequestDto updateDto)
    {
        var updatedFlight = await _flightService.UpdateFlightAsync(flightId, updateDto);
        if (updatedFlight == null)
        {
            return NotFound();
        }
        
        return Ok(FlightMapper.ToFlightDto(updatedFlight));
    }

    [HttpDelete("{flightId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFlight(int flightId)
    {
        await _flightService.DeleteFlightAsync(flightId);
        return NoContent();
    }
}