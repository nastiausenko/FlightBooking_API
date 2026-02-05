using FlightBooking.Domain;
using FlightBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Data;

public class FlightBookingDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public FlightBookingDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<Seat> Seats { get; set; }
}