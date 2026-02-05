using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Application.Dtos.Auth;

public class RegisterDto
{
    public required string Username { get; set; }
  
    public required string Password { get; set; }
 
    [EmailAddress]
    public required string Email { get; set; }
}