using FlightBooking.Application.Dtos.Auth;

namespace FlightBooking.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUserDto user, IEnumerable<string> roles);
}