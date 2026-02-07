namespace FlightBooking.Application.Exceptions;

public class BookingNotFoundException(int bookingId) : Exception($"Booking with id {bookingId} not found");
