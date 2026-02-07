namespace FlightBooking.Application.Exceptions.Booking;

public class BookingNotFoundException(int bookingId) : Exception($"Booking with id {bookingId} not found");
