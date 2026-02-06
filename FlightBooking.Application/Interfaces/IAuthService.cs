using FlightBooking.Application.Dtos.Auth;

namespace FlightBooking.Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
}