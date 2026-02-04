using FlightBooking.Appication.Dtos.Flight;
using FlightBooking.Appication.Mappers;
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
}