namespace FlightBooking.Application.Exceptions.Flight;

public class FlightAlreadyExistsException(string flightNumber) : Exception($"Flight with number \'{flightNumber}\' already exists");
