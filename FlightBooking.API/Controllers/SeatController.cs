using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/")]
[Authorize(Roles = "Admin")]
public class SeatController : ControllerBase
{
    private readonly ISeatService _seatService;
    
    public SeatController(ISeatService seatService)
    {
        _seatService = seatService;
    }

    [HttpPost("/api/flights/{flightId}/seats")]
    public async Task<IActionResult> AddSeat(int flightId, [FromBody] SeatRequestDto requestDto)
    {
        var seat = SeatMapper.ToSeat(requestDto);
        await _seatService.AddSeatToFlightAsync(flightId, seat);
        
        var dto = SeatMapper.ToSeatDto(seat);
        return CreatedAtAction(nameof(GetSeatById), new { id = dto.Id }, dto);
    }

    [HttpPut("seats/{id}")]
    public async Task<IActionResult> UpdateSeat(int id, [FromBody] SeatRequestDto seatDto)
    {
        var seat = await _seatService.UpdateSeatAsync(id, seatDto);
        if (seat == null)
        {
            return NotFound();
        }
        
        return Ok(SeatMapper.ToSeatDto(seat));
    }

    [HttpGet("seats/{id}")]
    public async Task<IActionResult> GetSeatById(int id)
    {
        var result = await _seatService.GetSeatByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(SeatMapper.ToSeatDto(result));
    }

    [HttpGet("flights/{flightId}/seats")]
    public async Task<ActionResult<IEnumerable<SeatDto>>> GetSeatByFlightId(int flightId)
    {
        var seats = await _seatService.GetAllByFlightIdAsync(flightId);
        var dto = seats.Select(SeatMapper.ToSeatDto);
        return Ok(dto);
    }

    [HttpDelete("seats/{id}")]
    public async Task<IActionResult> DeleteSeat(int id)
    {
        await _seatService.DeleteSeatAsync(id);
        return NoContent();
    }
}