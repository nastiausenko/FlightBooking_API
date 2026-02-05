using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain;
using FlightBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly FlightBookingDbContext _dbContext;

    public BookingService(FlightBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Booking> CreateBookingAsync(int userId, BookingRequestDto dto)
    {
        if (dto.BookingSeats == null || !dto.BookingSeats.Any())
        {
            throw new ArgumentException("No seats provided for booking");
        }

        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var booking = new Booking
        {
            User = user,          
            UserId = user.Id,
            BookingDate = DateTime.UtcNow,
            IsCancelled = false,
            BookingSeats = dto.BookingSeats.Select(BookingSeatMapper.ToBookingSeat).ToList()
        };

        var seatIds = booking.BookingSeats.Select(s => s.SeatId).ToList();
        var seats = await _dbContext.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync();

        if (seats.Count != seatIds.Count)
            throw new InvalidOperationException("One or more seats not found");

        decimal totalPrice = 0;

        foreach (var seat in seats)
        {
            if (seat.Status != SeatStatus.Available)
            {
                throw new InvalidOperationException($"Seat {seat.SeatNumber} is not available");
            }

            seat.Status = SeatStatus.Booked;

            var bookingSeat = booking.BookingSeats.First(bs => bs.SeatId == seat.Id);

            bookingSeat.Seat = seat;
            bookingSeat.Price = seat.Price;

            totalPrice += seat.Price;
        }

        booking.TotalPrice = totalPrice;

        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync();

        return booking;
    }

    public Task<Booking> CancelBookingAsync(int bookingId)
    {
        throw new NotImplementedException();
    }

    public Task<Booking> CancelBookingByAdminAsync(int? bookingId, int? userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Booking>?> GetUserBookingsAsync(int userId)
    {
        throw new NotImplementedException();
    }
}