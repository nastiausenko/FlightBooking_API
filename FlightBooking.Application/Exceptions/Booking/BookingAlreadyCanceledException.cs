namespace FlightBooking.Application.Exceptions.Booking;

public class BookingAlreadyCanceledException(int bookingId) : Exception($"Booking with id {bookingId} is already canceled");