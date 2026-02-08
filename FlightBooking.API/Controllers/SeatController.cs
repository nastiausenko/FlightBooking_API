using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/")]
[Authorize(Roles = "Admin")]
public class SeatController(ISeatService seatService) : ControllerBase
{
    /// <summary>
    /// Adds a new seat to a flight.
    /// </summary>
    /// <param name="flightId">ID of the flight to add the seat to.</param>
    /// <param name="requestDto">Seat data.</param>
    /// <returns>The created seat as SeatDto.</returns>
    [HttpPost("flights/{flightId:int}/seats")]
    public async Task<IActionResult> AddSeat(int flightId, [FromBody] SeatRequestDto requestDto)
    {
        var seat = SeatMapper.ToSeat(requestDto);
        await seatService.AddSeatToFlightAsync(flightId, seat);
        
        var dto = SeatMapper.ToSeatDto(seat);
        return CreatedAtAction(nameof(GetSeatById), new { seatId = dto.Id }, dto);
    }

    /// <summary>
    /// Updates an existing seat.
    /// </summary>
    /// <param name="seatId">ID of the seat to update.</param>
    /// <param name="seatDto">Updated seat data.</param>
    /// <returns>The updated seat as SeatDto.</returns>
    [HttpPut("seats/{seatId:int}")]
    public async Task<IActionResult> UpdateSeat(int seatId, [FromBody] SeatRequestDto seatDto)
    {
        var seat = await seatService.UpdateSeatAsync(seatId, seatDto);
        return Ok(SeatMapper.ToSeatDto(seat));
    }

    /// <summary>
    /// Retrieves a seat by its ID.
    /// </summary>
    /// <param name="seatId">ID of the seat.</param>
    /// <returns>The seat as SeatDto.</returns>
    [HttpGet("seats/{seatId:int}")]
    public async Task<IActionResult> GetSeatById(int seatId)
    {
        var result = await seatService.GetSeatByIdAsync(seatId);
        
        return Ok(SeatMapper.ToSeatDto(result));
    }

    /// <summary>
    /// Retrieves all seats for a specified flight.
    /// </summary>
    /// <param name="flightId">ID of the flight.</param>
    /// <returns>List of seats as SeatDto.</returns>
    [HttpGet("flights/{flightId:int}/seats")]
    public async Task<ActionResult<IEnumerable<SeatDto>>> GetSeatByFlightId(int flightId)
    {
        var seats = await seatService.GetAllByFlightIdAsync(flightId);
        var dto = seats.Select(SeatMapper.ToSeatDto);
        return Ok(dto);
    }

    /// <summary>
    /// Deletes a seat by its ID.
    /// </summary>
    /// <param name="seatId">ID of the seat to delete.</param>
    /// <returns>No content.</returns>
    [HttpDelete("seats/{seatId:int}")]
    public async Task<IActionResult> DeleteSeat(int seatId)
    {
        await seatService.DeleteSeatAsync(seatId);
        return NoContent();
    }
}