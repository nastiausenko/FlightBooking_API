namespace FlightBooking.Domain;

public class User
{
    public int Id { get; set; }
    
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    
    public Role Role { get; set; } = Role.Passenger;
    
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

