namespace FlightBooking.Application.Services;

public class ForbiddenException(string message) : Exception(message);