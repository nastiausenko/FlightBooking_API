using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Application.Dtos.Flight;

public class UpdateFlightRequestDto : IValidatableObject
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