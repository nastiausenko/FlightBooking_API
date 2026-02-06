using FlightBooking.Application.Dtos.Auth;
using FlightBooking.Application.Interfaces;
using FlightBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            throw new ApplicationException("An error occured while registering the user.");
        }
        
        await _userManager.AddToRoleAsync(user, "Passenger");
        return await _tokenService.CreateTokenAsync(user);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            throw new UnauthorizedAccessException();

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException();

        return await _tokenService.CreateTokenAsync(user);
    }
}