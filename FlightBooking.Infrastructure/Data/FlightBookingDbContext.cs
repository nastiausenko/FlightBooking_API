using FlightBooking.Domain.Models;
using FlightBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Data;

public class FlightBookingDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<Seat> Seats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationRole>().HasData(
            new ApplicationRole { Id = 1, Name = "Passenger", NormalizedName = "PASSENGER" },
            new ApplicationRole { Id = 2, Name = "Admin", NormalizedName = "ADMIN" }
        );
        
        modelBuilder.Entity<Flight>()
            .HasIndex(f => f.FlightNumber)
            .IsUnique();
        
        modelBuilder.Entity<Seat>()
            .HasIndex(seat => new {seat.FlightId, seat.SeatNumber})
            .IsUnique();
        
        modelBuilder.Entity<BookingSeat>()
            .HasIndex(bs => new { bs.BookingId, bs.SeatId })
            .IsUnique();
    }
}