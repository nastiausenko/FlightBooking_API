namespace FlightBooking.Application.Exceptions.Auth;

public class ForbiddenException(string message) : Exception(message);