namespace FlightBooking.Application.Exceptions;

public class BookingNotFoundException : Exception
{
    public int BookingId { get; }

    public BookingNotFoundException(int bookingId) : base($"Booking with id {bookingId} not found")
    {
        BookingId = bookingId;
    }
}