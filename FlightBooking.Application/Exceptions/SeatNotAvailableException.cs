namespace FlightBooking.Application.Exceptions;

public class SeatNotAvailableException : Exception
{
    public int SeatId { get; }

    public SeatNotAvailableException(int seatId) : base($"Seat with id {seatId} is not available")
    {
        SeatId = seatId;
    }
}