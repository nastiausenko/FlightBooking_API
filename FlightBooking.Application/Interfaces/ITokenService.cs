using FlightBooking.Infrastructure.Identity;

namespace FlightBooking.Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}