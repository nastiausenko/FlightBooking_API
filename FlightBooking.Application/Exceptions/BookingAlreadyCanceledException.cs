namespace FlightBooking.Application.Exceptions;

public class BookingAlreadyCanceledException(int bookingId) : Exception($"Booking with id {bookingId} is already canceled");