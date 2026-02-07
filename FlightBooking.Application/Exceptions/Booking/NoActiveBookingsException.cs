namespace FlightBooking.Application.Exceptions.Booking;

public class NoActiveBookingsException(int userId) : Exception($"User with id {userId} has no active bookings");