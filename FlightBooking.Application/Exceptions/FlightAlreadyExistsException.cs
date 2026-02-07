namespace FlightBooking.Application.Exceptions;

public class FlightAlreadyExistsException(string flightName) : Exception($"Flight with name {flightName} already exists");
