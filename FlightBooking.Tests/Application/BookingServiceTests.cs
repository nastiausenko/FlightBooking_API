using FlightBooking.Application.Dtos.Booking;
using FlightBooking.Application.Dtos.BookingSeat;
using FlightBooking.Application.Exceptions.Auth;
using FlightBooking.Application.Exceptions.Booking;
using FlightBooking.Application.Exceptions.Seat;
using FlightBooking.Application.Services;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using Moq;

namespace FlightBooking.Tests.Application;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepository = new();
    private readonly Mock<ISeatRepository> _seatRepository = new();
    private readonly BookingService _service;
    
    public BookingServiceTests()
    {
        _service = new BookingService(_bookingRepository.Object, _seatRepository.Object);
    }
    
    [Fact]
    public async Task CreateBookingAsync_SeatsAvailable_ReturnsCreatedBooking()
    {
        // Arrange
        var dto = CreateBookingRequestDto(1, 2);

        _seatRepository.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<Seat> 
            {
                CreateSeat(1, "1", 1000, SeatStatus.Available),
                CreateSeat(2, "2", 1000,  SeatStatus.Available)
            });
        
        // Act
        var booking = await _service.CreateBookingAsync(10, dto);
        
        // Assert
        Assert.NotNull(booking);
        Assert.Equal(10, booking.UserId);
        Assert.Equal(2000, booking.TotalPrice);

        Assert.Collection(booking.BookingSeats,
            bs => Assert.Equal(1, bs.SeatId),
            bs => Assert.Equal(2, bs.SeatId));

        Assert.All(booking.BookingSeats, bs => Assert.Equal(SeatStatus.Booked, bs.Seat.Status));
    }

    [Fact]
    public async Task CreateBookingAsync_SeatNotExist_ThrowsSeatNotFoundException()
    {
        // Arrange
        var dto = CreateBookingRequestDto(1, 2);
        _seatRepository.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Seat>());
        
        // Act
        var act = async () => await _service.CreateBookingAsync(10, dto);
        
        // Assert
        await Assert.ThrowsAsync<SeatNotFoundException>(act);
    }

    [Fact]
    public async Task CreateBookingAsync_SeatAlreadyBooked_ThrowsSeatNotAvailableException()
    {
        // Arrange 
        var dto = CreateBookingRequestDto(1, 2);
        
        _seatRepository.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<Seat> 
            {
                CreateSeat(1, "1", 1000, SeatStatus.Booked),
                CreateSeat(2, "2", 1000,  SeatStatus.Available)
            });
        
        // Act
        
        var act = async () => await _service.CreateBookingAsync(10, dto);

        // Assert
        await Assert.ThrowsAsync<SeatNotAvailableException>(act);
    }
    
    [Fact]
    public async Task CancelBookingAsync_ValidOwner_ReturnsCanceledBooking()
    {
        // Arrange 
        var booking = CreateActiveBooking(1, 10,  CreateSeat(1, "1", 1000, SeatStatus.Booked));
        _bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);
        
        // Act
        var result = await _service.CancelBookingAsync(1, 10, false);

        // Assert
        Assert.True(result.IsCancelled);
        Assert.All(result.BookingSeats, bs =>
        {
            Assert.True(bs.IsCancelled);
            Assert.Equal(SeatStatus.Available, bs.Seat.Status);
        });
        
        _bookingRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CancelBookingAsync_NotOwnerAndNotAdmin_ThrowsForbiddenException()
    {
        // Arrange 
        var booking = CreateActiveBooking(1, 99, CreateSeat(1, "1", 1000, SeatStatus.Booked));
        _bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);
        
        // Act
        var act = async() => await _service.CancelBookingAsync(1, 10, false);

        // Assert
        await Assert.ThrowsAsync<ForbiddenException>(act);
    }
    
    [Fact]
    public async Task CancelBookingAsync_AlreadyCanceled_ThrowsBookingAlreadyCanceledException()
    {
        // Arrange 
        var booking = CreateActiveBooking(1, 10, CreateSeat(1, "1", 1000, SeatStatus.Booked));
        booking.IsCancelled = true;
        
        _bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);
        
        // Act
        var act = async() => await _service.CancelBookingAsync(1, 10, false);

        // Assert
        await Assert.ThrowsAsync<BookingAlreadyCanceledException>(act);
    }
    
    [Fact]
    public async Task CancelBookingAsync_BookingNotFound_ThrowsBookingNotFoundException()
    {
        // Arrange 
        _bookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Booking?)null);
        
        // Act
        var act = async() => await _service.CancelBookingAsync(1, 10, false);

        // Assert
        await Assert.ThrowsAsync<BookingNotFoundException>(act);
    }
    
    [Fact]
    public async Task CancelBookingAsync_ByAdmin_ReturnsCanceledBooking()
    {
        // Arrange 
        var booking = CreateActiveBooking(1, 99, CreateSeat(1, "1", 1000, SeatStatus.Booked));
        _bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);
        
        // Act
        var result = await _service.CancelBookingAsync(1, 10, true);

        // Assert
        Assert.True(result.IsCancelled);
        Assert.All(result.BookingSeats, bs =>
            {
                Assert.True(bs.IsCancelled);
                Assert.Equal(SeatStatus.Available, bs.Seat.Status);
            }
        );
    }

    [Fact]
    public async Task CancelUserBookingsByAdminAsync_ActiveBookingsExist_ReturnsCanceledBookings()
    {
        // Arrange 
        var bookings = new List<Booking>
        {
            CreateActiveBooking(1, 99, CreateSeat(1, "1", 100, SeatStatus.Booked)), 
            CreateActiveBooking(2, 99 , CreateSeat(2, "2", 100, SeatStatus.Booked)),
        };
        _bookingRepository.Setup(r => r.GetActiveByUserIdAsync(99)).ReturnsAsync(bookings);
        
        // Act
        var result = await _service.CancelUserBookingsByAdminAsync(99);

        // Assert
        Assert.All(result, booking => Assert.True(booking.IsCancelled));
        
        _bookingRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CancelUserBookingsByAdminAsync_NoActiveBookings_ThrowsNoActiveBookingsException()
    {
        // Arrange 
        _bookingRepository.Setup(r => r.GetActiveByUserIdAsync(99)).ReturnsAsync(new List<Booking>());
        
        // Act
        var act = async() => await _service.CancelUserBookingsByAdminAsync(99);

        // Assert
        await Assert.ThrowsAsync<NoActiveBookingsException>(act);
    }
    
    [Fact]
    public async Task GetUserBookingsAsync_ReturnsUserBookings()
    {
        // Arrange 
        var bookings = new List<Booking>
        {
            CreateActiveBooking(1, 99,CreateSeat(1, "1", 100, SeatStatus.Booked)), 
            CreateActiveBooking(2, 99, CreateSeat(2, "2", 100, SeatStatus.Booked)),
        };
        _bookingRepository.Setup(r => r.GetByUserIdAsync(99)).ReturnsAsync(bookings);
        
        // Act
        var result = await _service.GetUserBookingsAsync(99);

        // Assert
        Assert.Equal(bookings.Count, result.Count);
        Assert.All(result, b => Assert.Equal(99, b.UserId));
        
        _bookingRepository.Verify(r => r.GetByUserIdAsync(99), Times.Once);
    }

    private static BookingRequestDto CreateBookingRequestDto(params int[] seatIds)
    {
        return new BookingRequestDto
        {
            BookingSeats = seatIds.Select(id => new BookingSeatRequestDto { SeatId = id }).ToList()
        };
    }
    
    private static Seat CreateSeat(int id, string number, decimal price, SeatStatus status)
    {
        return new Seat
        { 
            Id = id,
            SeatNumber = number,
            Price = price,
            Status = status
        };
    }
    
    private static Booking CreateActiveBooking(int bookingId, int userId, params Seat[] seats)
    {
        return new Booking
        {
            Id = bookingId,
            UserId = userId,
            IsCancelled = false,
            BookingSeats = seats.Select(s => new BookingSeat { Seat = s, IsCancelled = false }).ToList()
        };
    }
}