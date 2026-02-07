using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Application.Dtos.Seat;

public class SeatRequestDto
{
    [Required]
    [StringLength(10)]
    public string SeatNumber { get; set; } = string.Empty;
    
    [Range(1, 10000)]
    public decimal Price { get; set; }
}