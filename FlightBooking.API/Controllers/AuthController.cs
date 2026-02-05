using FlightBooking.Application.Dtos.Auth;
using FlightBooking.Application.Interfaces;
using FlightBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService,  SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null)
        {
            return Unauthorized();
        }
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password,  false);

        if (!result.Succeeded)
        {
            return Unauthorized();
        }
        
        return Ok(new ResponseDto
        {
            Token = _tokenService.CreateToken(user)
        });
    }
}