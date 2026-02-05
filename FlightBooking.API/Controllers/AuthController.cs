using FlightBooking.Application.Dtos.Auth;
using FlightBooking.Application.Interfaces;
using FlightBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Passenger");
        if (!roleResult.Succeeded)
        {
            return BadRequest();
        }

        var token = _tokenService.CreateToken(user);
        var response = new ResponseDto
        {
            Token = token
        };

        return Ok(response);
    }
}