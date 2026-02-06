using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBooking.Domain.Models;

public class Booking
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }
    public int UserId { get; set; }
    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    public bool IsCancelled { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TotalPrice { get; set; } 
}