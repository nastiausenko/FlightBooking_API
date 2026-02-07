namespace FlightBooking.Application.Services;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}