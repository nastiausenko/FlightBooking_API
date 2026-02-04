namespace FlightBooking.Domain;

public class Seat
{
    public int Id { get; set; }
    public required string SeatNumber { get; set; }
    public SeatStatus Status { get; set; } = SeatStatus.Available;
    
    public int FlightId { get; set; }
    public required Flight Flight { get; set; }
    
    public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}