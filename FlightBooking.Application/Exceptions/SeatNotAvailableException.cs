namespace FlightBooking.Application.Exceptions;

public class SeatNotAvailableException(int seatId) : Exception($"Seat with id {seatId} is not available");