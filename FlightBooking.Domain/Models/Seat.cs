using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBooking.Domain.Models;

public class Seat
{
    public int Id { get; set; }
    public required string SeatNumber { get; set; }
    public SeatStatus Status { get; set; } = SeatStatus.Available;
    [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }
    public int FlightId { get; set; }
    public Flight Flight { get; set; } = null!;
    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}