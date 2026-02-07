using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Exceptions;
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
        var booking = BookingMapper.ToBooking(dto);
        booking.UserId = userId;
        booking.BookingDate = DateTime.UtcNow;

        var seatIds = booking.BookingSeats.Select(s => s.SeatId).ToList();
        var seats = await _seatRepository.GetByIdsAsync(seatIds);
        
        var foundSeatIds = seats.Select(s => s.Id).ToHashSet();
        var missingSeatIds = seatIds.Where(id => !foundSeatIds.Contains(id)).ToList();

        if (missingSeatIds.Count > 0)
        {
            throw new SeatNotFoundException(missingSeatIds.ToArray());
        }

        decimal totalPrice = 0;

        foreach (var seat in seats)
        {
            if (seat.Status != SeatStatus.Available)
            {
                throw new SeatNotAvailableException(seat.Id);
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

    public async Task<Booking> CancelBookingAsync(int bookingId, int userId, bool isAdmin)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            throw new BookingNotFoundException(bookingId);
        }

        if (booking.IsCancelled)
        {
            throw new BookingAlreadyCanceledException(bookingId);
        }

        if (!isAdmin && booking.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to cancel this booking");
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
        if (bookings.Count == 0)
        {
           throw new NoActiveBookingsException(userId);
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

    public async Task<Booking?> GetBookingByIdAsync(int bookingId) => 
        await _bookingRepository.GetByIdAsync(bookingId)
        ?? throw new BookingNotFoundException(bookingId);
}