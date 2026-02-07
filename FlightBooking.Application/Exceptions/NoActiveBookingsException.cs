namespace FlightBooking.Application.Exceptions;

public class NoActiveBookingsException : Exception
{
    public int UserId { get; }

    public NoActiveBookingsException(int userId) : base($"User with id {userId} has no active bookings")
    {
        UserId = userId;
    }
}