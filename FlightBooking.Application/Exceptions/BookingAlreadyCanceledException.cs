namespace FlightBooking.Application.Exceptions;

public class BookingAlreadyCanceledException : Exception
{
    public int BookingId { get; }

    public BookingAlreadyCanceledException(int bookingId) : base($"Booking with id {bookingId} is already canceled")
    {
        BookingId = bookingId;
    }
}