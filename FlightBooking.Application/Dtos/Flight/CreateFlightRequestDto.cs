using System.ComponentModel.DataAnnotations;
using FlightBooking.Application.Dtos.Seat;

namespace FlightBooking.Application.Dtos.Flight;

public class CreateFlightRequestDto : IValidatableObject
{
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string FlightNumber { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string From { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string To { get; set; } = string.Empty;
    
    [Required]
    public DateTime Departure { get; set; }
    
    [Required]
    public DateTime Arrival { get; set; }
    
    [MinLength(1, ErrorMessage = "Flight must contain at least one seat")]
    public List<SeatRequestDto>? Seats { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Arrival <= Departure)
        {
            yield return new ValidationResult(
                "Arrival must be later than Departure",
                new[] { nameof(Arrival), nameof(Departure) }
            );
        }
    }
}