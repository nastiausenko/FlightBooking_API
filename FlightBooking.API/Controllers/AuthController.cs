using FlightBooking.Application.Dtos.Auth;
using FlightBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var token = await authService.RegisterAsync(dto);
        return Ok(new ResponseDto { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await authService.LoginAsync(dto);
        return Ok(new ResponseDto { Token = token });
    }
}