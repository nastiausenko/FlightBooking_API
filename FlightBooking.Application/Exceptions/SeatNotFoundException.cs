namespace FlightBooking.Application.Exceptions;

public class SeatNotFoundException(params int[] missingSeatIds) 
    : Exception($"Seats not found with id: {string.Join(", ", missingSeatIds)}");