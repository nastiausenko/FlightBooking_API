namespace FlightBooking.Application.Exceptions.Seat;

public class SeatNotAvailableException(int seatId) : Exception($"Seat with id {seatId} is not available");