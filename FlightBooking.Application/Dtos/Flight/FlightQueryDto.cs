using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Application.Dtos.Flight;

public class FlightQueryDto
{
    [StringLength(50, MinimumLength = 2)]
    public string? From { get; set; }
    
    [StringLength(50, MinimumLength = 2)]
    public string? To { get; set; }
}