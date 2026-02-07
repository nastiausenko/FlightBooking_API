using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ISeatRepository _seatRepository;

    public BookingService(IBookingRepository bookingRepository,  ISeatRepository seatRepository)
    {
        _bookingRepository  = bookingRepository;
        _seatRepository = seatRepository;
    }

    public async Task<Booking> CreateBookingAsync(int userId, BookingRequestDto dto)
    {
        if (dto.BookingSeats == null || !dto.BookingSeats.Any())
        {
            throw new ArgumentException("No seats provided for booking");
        }

        var booking = new Booking
        {
            UserId = userId,
            BookingDate = DateTime.UtcNow,
            IsCancelled = false,
            BookingSeats = dto.BookingSeats.Select(BookingSeatMapper.ToBookingSeat).ToList()
        };

        var seatIds = booking.BookingSeats.Select(s => s.SeatId).ToList();
        var seats = await _seatRepository.GetByIdsAsync(seatIds);

        if (seats == null)
        {
            throw new KeyNotFoundException("Seat not found");
        }

        if (seats.Count != seatIds.Count)
        {
            throw new InvalidOperationException("One or more seats not found");
        }

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

        await _bookingRepository.AddAsync(booking);
        return booking;
    }

    public async Task<Booking> CancelBookingAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found");
        }

        if (booking.IsCancelled)
        {
            throw new InvalidOperationException("Booking is already canceled");
        }
        
        booking.IsCancelled = true;

        foreach (var bookingSeat in booking.BookingSeats)
        {
            bookingSeat.IsCancelled = true;
            bookingSeat.Seat.Status = SeatStatus.Available;
        }

        await _bookingRepository.SaveChangesAsync();
        return booking;
    }

    public async Task<List<Booking>> CancelUserBookingsByAdminAsync(int userId)
    {
        var bookings = await _bookingRepository.GetActiveByUserIdAsync(userId);
        if (!bookings.Any())
        {
           throw new KeyNotFoundException("No active bookings found");
        }
        
        foreach (var booking in bookings)
        {
            booking.IsCancelled = true;
            foreach (var bookingSeat in booking.BookingSeats)
            {
                bookingSeat.IsCancelled = true;
                bookingSeat.Seat.Status = SeatStatus.Available;
            }
        }
        await _bookingRepository.SaveChangesAsync();
        return bookings;
    }

    public async Task<List<Booking>> GetUserBookingsAsync(int userId)
    {
        return await _bookingRepository.GetByUserIdAsync(userId);
    }

    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        return await _bookingRepository.GetByIdAsync(bookingId);
    }
}