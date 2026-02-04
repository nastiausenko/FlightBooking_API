using FlightBooking.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Data;

public class FlightBookingDbContext : DbContext
{
    public FlightBookingDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<Seat> Seats { get; set; }
}