using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBooking.Domain;

public class BookingSeat
{
    public int Id { get; set; }
    
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    
    public int SeatId { get; set; }
    public Seat Seat { get; set; } = null!;
    
    public bool IsCancelled { get; set; } = false; 
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } 
}