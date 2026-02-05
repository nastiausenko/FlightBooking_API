using FlightBooking.Infrastructure.Identity;

namespace FlightBooking.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}