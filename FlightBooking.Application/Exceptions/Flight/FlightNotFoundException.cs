namespace FlightBooking.Application.Exceptions.Flight;

public class FlightNotFoundException(int flightId) : Exception($"Flight with id {flightId} not found");
