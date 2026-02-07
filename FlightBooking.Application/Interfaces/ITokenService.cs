using FlightBooking.Application.Dtos.Auth;

namespace FlightBooking.Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUserDto user, IEnumerable<string> roles);
}