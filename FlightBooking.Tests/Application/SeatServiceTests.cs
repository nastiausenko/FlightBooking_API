using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Application.Exceptions.Flight;
using FlightBooking.Application.Exceptions.Seat;
using FlightBooking.Application.Services;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using Moq;

namespace FlightBooking.Tests.Application;

public class SeatServiceTests
{
    private readonly Mock<ISeatRepository> _seatRepository = new();
    private readonly Mock<IFlightRepository> _flightRepository = new();
    private readonly SeatService _service;

    public SeatServiceTests()
    {
        _service = new SeatService(_seatRepository.Object, _flightRepository.Object);
    }

    [Fact]
    public async Task AddSeatToFlightAsync_ValidInput_ReturnsCreatedSeat()
    {
        // Arrange
        var seat = CreateSeat(1, "12B");
        _flightRepository.Setup(fr => fr.ExistsByIdAsync(It.IsAny<int>())).ReturnsAsync(true);
        _seatRepository.Setup(sr => sr.ExistsByFlightIdAndNumberAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);
        
        // Act
        var result = await _service.AddSeatToFlightAsync(1, seat);
        
        Assert.Equal(1, result.FlightId);
        Assert.Equal("12B", result.SeatNumber);
        
        // Assert
        _seatRepository.Verify(sr => sr.AddAsync(It.IsAny<Seat>()), Times.Once);
    }
    
    [Fact]
    public async Task AddSeatToFlightAsync_FlightNotFound_ThrowsFlightNotFoundException()
    {
        //Arrange
        var seat = CreateSeat(1, "12B");
        _flightRepository.Setup(fr => fr.ExistsByIdAsync(It.IsAny<int>())).ReturnsAsync(false);
        
        // Act
        var act = async() => await _service.AddSeatToFlightAsync(1, seat);

        // Assert
        await Assert.ThrowsAsync<FlightNotFoundException>(act);
    }

    [Fact]
    public async Task AddSeatToFlightAsync_SeatNumberExistsOnFlight_ThrowsSeatAlreadyExistsException()
    {
        // Arrange
        var seat = CreateSeat(1, "12B");
        _flightRepository.Setup(fr => fr.ExistsByIdAsync(It.IsAny<int>())).ReturnsAsync(true);
        _seatRepository.Setup(sr => sr.ExistsByFlightIdAndNumberAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
     
        // Act
        var act = async() => await _service.AddSeatToFlightAsync(1, seat);

        // Assert
        await Assert.ThrowsAsync<SeatAlreadyExistsException>(act);
    }
    
    [Fact]
    public async Task UpdateSeatAsync_ValidInput_ReturnsUpdatedSeat()
    {
        // Arrange
        var seat = CreateSeat(1, "12B");
        seat.FlightId = 42;

        _seatRepository.Setup(sr => sr.GetByIdAsync(1)).ReturnsAsync(seat);
        _seatRepository.Setup(sr => sr.GetByFlightIdAsync(42)).ReturnsAsync(new List<Seat> { seat });

        var dto = new SeatRequestDto
        {
            SeatNumber = "12B",
            Price = 1500
        };

        // Act
        var result = await _service.UpdateSeatAsync(1, dto);

        // Assert
        Assert.Equal("12B", result.SeatNumber);
        Assert.Equal(1500, result.Price);
        
        _seatRepository.Verify(sr => sr.UpdateAsync(seat), Times.Once);
    }
    
    
    [Fact]
    public async Task UpdateSeatAsync_SeatNotFound_ThrowsSeatNotFoundException()
    {
        // Arrange
        _seatRepository.Setup(sr => sr.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Seat?)null);
        var dto = new SeatRequestDto { SeatNumber = "12B", Price = 1000 };
        
        // Act
        var act = async () => await _service.UpdateSeatAsync(1, dto);

        // Assert
        await Assert.ThrowsAsync<SeatNotFoundException>(act);
    }

    [Fact]
    public async Task UpdateSeatAsync_SeatNumberExistsOnFlight_ThrowsSeatAlreadyExistsException()
    {
        // Arrange
        var seat = CreateSeat(1, "12B");
        seat.FlightId = 42;

        var otherSeat = CreateSeat(2, "12C");
        seat.FlightId = 42;

        _seatRepository.Setup(sr => sr.GetByIdAsync(1)).ReturnsAsync(seat);
        _seatRepository.Setup(sr => sr.GetByFlightIdAsync(42)).ReturnsAsync(new List<Seat> { seat, otherSeat });

        var dto = new SeatRequestDto { SeatNumber = "12C", Price = 1000 };
        
        // Act
        var act = async () => await _service.UpdateSeatAsync(1, dto);

        // Assert
        await Assert.ThrowsAsync<SeatAlreadyExistsException>(act);
    }

    [Fact]
    public async Task DeleteSeatAsync_ExistingSeat_DeletesSeatAndUpdatesBookingPrice()
    {
        // Arrange
        var seat = CreateSeat(1, "12B");
        seat.BookingSeats = new List<BookingSeat>
        {
            new()
            {
                Price = 100,
                Booking = new Booking { TotalPrice = 500 }
            }
        };

        _seatRepository.Setup(sr => sr.GetByIdAsync(1)).ReturnsAsync(seat);
        _seatRepository.Setup(sr => sr.DeleteAsync(seat)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteSeatAsync(1);

        // Assert
        Assert.Equal(400, seat.BookingSeats.First().Booking.TotalPrice); 
        
        _seatRepository.Verify(sr => sr.DeleteAsync(seat), Times.Once);
    }

    [Fact]
    public async Task DeleteSeatAsync_NonExistingSeat_DoesNothing()
    {
        // Arrange
        _seatRepository.Setup(sr => sr.GetByIdAsync(1)).ReturnsAsync((Seat?)null);
        
        // Act
        var ex = await Record.ExceptionAsync(() => _service.DeleteSeatAsync(1));

        // Assert
        Assert.Null(ex);
        _seatRepository.Verify(sr => sr.DeleteAsync(It.IsAny<Seat>()), Times.Never);
    }
    
    [Fact]
    public async Task GetSeatByIdAsync_ExistingSeat_ReturnsSeat()
    {
        // Arrange
        var seat = CreateSeat(1, "12B");
        _seatRepository.Setup(sr => sr.GetByIdAsync(1)).ReturnsAsync(seat);

        // Act
        var result = await _service.GetSeatByIdAsync(1);

        // Assert
        Assert.Equal(seat, result);
    }

    [Fact]
    public async Task GetSeatByIdAsync_SeatNotFound_ThrowsSeatNotFoundException()
    {
        // Arrange
        _seatRepository.Setup(sr => sr.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Seat?)null);

        // Act
        var act = async () => await _service.GetSeatByIdAsync(1);

        // Assert
        await Assert.ThrowsAsync<SeatNotFoundException>(act);
    }
    
    [Fact]
    public async Task GetAllByFlightIdAsync_FlightExists_ReturnsSeats()
    {
        // Arrange
        var seats = new List<Seat> { CreateSeat(1, "12B"), CreateSeat(2, "12C") };
        _flightRepository.Setup(fr => fr.ExistsByIdAsync(42)).ReturnsAsync(true);
        _seatRepository.Setup(sr => sr.GetByFlightIdAsync(42)).ReturnsAsync(seats);

        // Act
        var result = await _service.GetAllByFlightIdAsync(42);

        // Assert
        Assert.Equal(seats.Count, result.Count);
    }

    [Fact]
    public async Task GetAllByFlightIdAsync_FlightNotFound_ThrowsFlightNotFoundException()
    {
        // Arrange
        _flightRepository.Setup(fr => fr.ExistsByIdAsync(42)).ReturnsAsync(false);

        // Act
        var act = async () => await _service.GetAllByFlightIdAsync(42);

        // Assert
        await Assert.ThrowsAsync<FlightNotFoundException>(act);
    }
    
    private static Seat CreateSeat(int id, string seatNumber)
    {
        return new Seat
        {
            Id = id,
            SeatNumber = seatNumber,
            Price = 1000,
            Status = SeatStatus.Available
        };
    }
}