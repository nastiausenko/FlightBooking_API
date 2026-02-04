namespace FlightBooking.Application.Dtos.Flight;

public class UpdateFlightRequest
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    
    public DateTime Departure { get; set; }
    public DateTime Arrival { get; set; }
}