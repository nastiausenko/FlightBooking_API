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
            bookingSeat.IsCancelled = false;

            totalPrice += seat.Price;
        }

        booking.TotalPrice = totalPrice;

        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync();

        return booking;
    }

    public async Task<Booking> CancelBookingAsync(int bookingId)
    {
        var  booking = await _dbContext.Bookings
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
        
        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found");
        }

        if (booking.IsCancelled)
        {
            throw new InvalidOperationException("Booking is already canceled");
        }
        
        booking.IsCancelled = true;

        foreach (var bookingSeat in  booking.BookingSeats )
        {
            bookingSeat.IsCancelled = true;
            bookingSeat.Seat.Status = SeatStatus.Available;
        }
        
        await _dbContext.SaveChangesAsync();
        return booking;
    }

    public async Task<List<Booking>> CancelBookingByAdminAsync(int? bookingId, int? userId)
    {
        if (!bookingId.HasValue && !userId.HasValue)
        {
            throw new ArgumentException("Either bookingId or userId should be provided");
        }

        var bookingsToCancel = new List<Booking>();
        if (bookingId.HasValue)
        {
            var booking = await _dbContext.Bookings
                .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.Id == bookingId.Value);

            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found");
            }

            if (!booking.IsCancelled)
            {
                booking.IsCancelled = true;
                foreach (var bookingSeat in booking.BookingSeats)
                {
                    bookingSeat.IsCancelled = true;
                    bookingSeat.Seat.Status = SeatStatus.Available;
                }
                bookingsToCancel.Add(booking);
            }
        }
        else if (userId.HasValue)
        {
            var userExists = await _dbContext.Users.AnyAsync(b => b.Id == userId.Value);
            if (!userExists)
            {
                throw new KeyNotFoundException("User not found");
            }
            
            var userBookings = await _dbContext.Bookings
                .Where(b => b.UserId == userId.Value && !b.IsCancelled)
                .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
                .ToListAsync();

            foreach (var booking in userBookings)
            {
                booking.IsCancelled = true;
                foreach (var bookingSeat in booking.BookingSeats)
                {
                    bookingSeat.IsCancelled = true;
                    bookingSeat.Seat.Status = SeatStatus.Available;
                }
                bookingsToCancel.Add(booking);
            }
        }
        
        await _dbContext.SaveChangesAsync();
        return bookingsToCancel;
    }

    public async Task<List<Booking>> GetUserBookingsAsync(int userId)
    {
        var exists = await _dbContext.Users.AnyAsync(user => user.Id == userId);
        if (!exists)
        {
            throw new KeyNotFoundException("User not found");
        }
        
        return await _dbContext.Bookings.Where(b => b.UserId == userId)
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .ToListAsync();
    }
    
    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        return await _dbContext.Bookings
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }
}