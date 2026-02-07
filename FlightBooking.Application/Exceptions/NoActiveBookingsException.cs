namespace FlightBooking.Application.Exceptions;

public class NoActiveBookingsException(int userId) : Exception($"User with id {userId} has no active bookings");