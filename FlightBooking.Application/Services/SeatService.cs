using FlightBooking.Infrastructure.Data;

namespace FlightBooking.Application.Services;

public class SeatService
{
    private readonly FlightBookingDbContext _dbContext;

    public SeatService(FlightBookingDbContext context)
    {
        _dbContext = context;
    }
    
}