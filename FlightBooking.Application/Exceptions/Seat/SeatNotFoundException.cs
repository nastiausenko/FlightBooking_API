namespace FlightBooking.Application.Exceptions.Seat;

public class SeatNotFoundException(params int[] missingSeatIds) 
    : Exception($"Seats not found with id: {string.Join(", ", missingSeatIds)}");