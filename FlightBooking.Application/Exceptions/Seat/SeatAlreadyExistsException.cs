namespace FlightBooking.Application.Exceptions.Seat;

public class SeatAlreadyExistsException(string seatNumber, int flightId) 
    : Exception($"Seat with number \'{seatNumber}\' already exists on flight with id {flightId}"); 