using FlightBooking.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace FlightBooking.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}