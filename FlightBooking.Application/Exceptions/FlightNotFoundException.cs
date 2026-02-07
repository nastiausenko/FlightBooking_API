namespace FlightBooking.Application.Exceptions;

public class FlightNotFoundException : Exception
{
    public int FlightId { get; }

    public FlightNotFoundException(int flightId) : base($"Flight with id {flightId} not found")
    {
        FlightId = flightId;
    }
}