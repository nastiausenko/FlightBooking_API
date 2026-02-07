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

    [HttpPost("flights/{flightId:int}/seats")]
    public async Task<IActionResult> AddSeat(int flightId, [FromBody] SeatRequestDto requestDto)
    {
        var seat = SeatMapper.ToSeat(requestDto);
        await _seatService.AddSeatToFlightAsync(flightId, seat);
        
        var dto = SeatMapper.ToSeatDto(seat);
        return CreatedAtAction(nameof(GetSeatById), new { seatId = dto.Id }, dto);
    }

    [HttpPut("seats/{seatId:int}")]
    public async Task<IActionResult> UpdateSeat(int seatId, [FromBody] SeatRequestDto seatDto)
    {
        var seat = await _seatService.UpdateSeatAsync(seatId, seatDto);
        return Ok(SeatMapper.ToSeatDto(seat));
    }

    [HttpGet("seats/{seatId:int}")]
    public async Task<IActionResult> GetSeatById(int seatId)
    {
        var result = await _seatService.GetSeatByIdAsync(seatId);
        
        return Ok(SeatMapper.ToSeatDto(result));
    }

    [HttpGet("flights/{flightId:int}/seats")]
    public async Task<ActionResult<IEnumerable<SeatDto>>> GetSeatByFlightId(int flightId)
    {
        var seats = await _seatService.GetAllByFlightIdAsync(flightId);
        var dto = seats.Select(SeatMapper.ToSeatDto);
        return Ok(dto);
    }

    [HttpDelete("seats/{seatId:int}")]
    public async Task<IActionResult> DeleteSeat(int seatId)
    {
        await _seatService.DeleteSeatAsync(seatId);
        return NoContent();
    }
}