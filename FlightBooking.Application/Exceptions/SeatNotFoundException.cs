namespace FlightBooking.Application.Exceptions;

public class SeatNotFoundException : Exception
{
    public IReadOnlyList<int> MissingSeatIds { get; }

    public SeatNotFoundException(params int[] missingSeatIds)
        : base($"Seats not found with id: {string.Join(", ", missingSeatIds)}")
    {
        MissingSeatIds = missingSeatIds;
    }
}