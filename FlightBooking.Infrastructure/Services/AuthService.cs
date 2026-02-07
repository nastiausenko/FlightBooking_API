using FlightBooking.Application.Dtos.Auth;
using FlightBooking.Application.Exceptions;
using FlightBooking.Application.Interfaces;
using FlightBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager, ITokenService tokenService) : IAuthService
{
    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new UserRegistrationException(errors);
        }
        
        await userManager.AddToRoleAsync(user, "Passenger");
        
        return await GenerateToken(user);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email) 
                   ?? throw new UnauthorizedAccessException();

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException();
        }

        return await GenerateToken(user);
    }
    
    private async Task<string> GenerateToken(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var userDto = new AppUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };

        return tokenService.CreateToken(userDto, roles);
    }
}