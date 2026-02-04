using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBooking.Domain;

public class BookingSeat
{
    public int Id { get; set; }
    
    public int BookingId { get; set; }
    public required Booking Booking { get; set; }
    
    public int SeatId { get; set; }
    public required Seat Seat { get; set; }
    
    public bool IsCancelled { get; set; } = false; 
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } 
}