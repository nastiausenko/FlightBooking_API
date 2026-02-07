using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Exceptions.Auth;
using FlightBooking.Application.Exceptions.Booking;
using FlightBooking.Application.Exceptions.Seat;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

public class BookingService(IBookingRepository bookingRepository, ISeatRepository seatRepository) : IBookingService
{
    public async Task<Booking> CreateBookingAsync(int userId, BookingRequestDto dto)
    {
        var booking = BookingMapper.ToBooking(dto);
        booking.UserId = userId;
        booking.BookingDate = DateTime.UtcNow;

        var seatIds = booking.BookingSeats.Select(s => s.SeatId).ToList();
        var seats = await seatRepository.GetByIdsAsync(seatIds);
        
        ValidateSeatsExistence(seatIds, seats);

        booking.TotalPrice = AssignSeatsToBooking(booking, seats);

        await bookingRepository.AddAsync(booking);
        return booking;
    }

    public async Task<Booking> CancelBookingAsync(int bookingId, int userId, bool isAdmin)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId) ?? throw new BookingNotFoundException(bookingId);
        
        if (!isAdmin && booking.UserId != userId)
        {
            throw new ForbiddenException("You are not allowed to cancel this booking");
        }

        if (booking.IsCancelled)
        {
            throw new BookingAlreadyCanceledException(bookingId);
        }
        
        CancelBooking(booking);

        await bookingRepository.SaveChangesAsync();
        return booking;
    }

    public async Task<List<Booking>> CancelUserBookingsByAdminAsync(int userId)
    {
        var bookings = await bookingRepository.GetActiveByUserIdAsync(userId);
        if (bookings.Count == 0)
        {
           throw new NoActiveBookingsException(userId);
        }
        
        foreach (var booking in bookings)
        {
            CancelBooking(booking);
        }
        
        await bookingRepository.SaveChangesAsync();
        return bookings;
    }

    public async Task<List<Booking>> GetUserBookingsAsync(int userId) => await bookingRepository.GetByUserIdAsync(userId);

    public async Task<Booking?> GetBookingByIdAsync(int bookingId) => 
        await bookingRepository.GetByIdAsync(bookingId) ?? throw new BookingNotFoundException(bookingId);
    
    private static void ValidateSeatsExistence(List<int> requestedIds, List<Seat> foundSeats)
    {
        var foundSeatIds = foundSeats.Select(s => s.Id).ToHashSet();
        var missingSeatIds = requestedIds.Where(id => !foundSeatIds.Contains(id)).ToArray();
        if (missingSeatIds.Length > 0)
        {
            throw new SeatNotFoundException(missingSeatIds);
        }
    }
    
    private static decimal AssignSeatsToBooking(Booking booking, List<Seat> seats)
    {
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

        return totalPrice;
    }

    private static void CancelBooking(Booking booking)
    {
        booking.IsCancelled = true;
        foreach (var bookingSeat in booking.BookingSeats)
        {
            bookingSeat.IsCancelled = true;
            bookingSeat.Seat.Status = SeatStatus.Available;
        }
    }
}