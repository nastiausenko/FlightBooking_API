namespace FlightBooking.Application.Exceptions;

public class FlightNotFoundException(int flightId) : Exception($"Flight with id {flightId} not found");
