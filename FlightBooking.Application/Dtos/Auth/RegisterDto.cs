using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Application.Dtos.Auth;

public class RegisterDto
{
    [Required]
    public string Username { get; set; }
  
    [Required]
    public string Password { get; set; }
 
    [EmailAddress]
    [Required]
    public string Email { get; set; }
}