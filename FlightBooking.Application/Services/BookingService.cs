using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Exceptions.Auth;
using FlightBooking.Application.Exceptions.Booking;
using FlightBooking.Application.Exceptions.Seat;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;

namespace FlightBooking.Application.Services;

/// <summary>
/// Service responsible for booking management logic:
/// creating bookings, cancelling bookings and retrieving user bookings. 
/// </summary>
/// <param name="bookingRepository"></param>
/// <param name="seatRepository"></param>
public class BookingService(IBookingRepository bookingRepository, ISeatRepository seatRepository) : IBookingService
{
    /// <summary>
    /// Creates a new booking for the specified user and books the selected seats
    /// </summary>
    /// <param name="userId">ID of the user who creates the booking.</param>
    /// <param name="dto">Booking request containing selected seat IDs.</param>
    /// <returns>The created booking.</returns>
    /// <exception cref="SeatNotFoundException">Thrown when one or more requested seats do not exist.</exception>
    /// <exception cref="SeatNotAvailableException">Thrown when one or more seats are not available for booking.</exception>
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

    /// <summary>
    /// Cancels a booking by its ID.
    /// User can cancel only their own booking unless they are an administrator.
    /// </summary>
    /// <param name="bookingId">ID of the booking to cancel.</param>
    /// <param name="userId">ID of the requesting user.</param>
    /// <param name="isAdmin">Indicates whether the user has administrator privileges.</param>
    /// <returns>The canceled booking.</returns>
    /// <exception cref="BookingNotFoundException">Thrown when the booking does not exist.</exception>
    /// <exception cref="ForbiddenException">Thrown when the user is not allowed to cancel the booking.</exception>
    /// <exception cref="BookingAlreadyCanceledException">Thrown when the booking has already been canceled.</exception>
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

    /// <summary>
    /// Cancels all active bookings of a specific user by administrator.
    /// </summary>
    /// <param name="userId">ID of the user whose bookings will be canceled.</param>
    /// <returns>List of canceled bookings.</returns>
    /// <exception cref="NoActiveBookingsException">Thrown when the user has no active bookings.</exception>
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

    /// <summary>
    /// Returns all bookings of a specific user.
    /// </summary>
    /// <param name="userId">ID of the user.</param>
    /// <returns>List of user bookings.</returns>
    public async Task<List<Booking>> GetUserBookingsAsync(int userId) => await bookingRepository.GetByUserIdAsync(userId);
    
    // Ensures that all requested seats exist
    private static void ValidateSeatsExistence(List<int> requestedIds, List<Seat> foundSeats)
    {
        var foundSeatIds = foundSeats.Select(s => s.Id).ToHashSet();
        var missingSeatIds = requestedIds.Where(id => !foundSeatIds.Contains(id)).ToArray();
        if (missingSeatIds.Length > 0)
        {
            throw new SeatNotFoundException(missingSeatIds);
        }
    }
    
    // Assigns available seats to the booking and calculates total price
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

    // Cancels the booking and releases all booked seats
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