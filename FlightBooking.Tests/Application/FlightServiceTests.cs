using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Exceptions.Flight;
using FlightBooking.Application.Services;
using FlightBooking.Domain.Interfaces;
using FlightBooking.Domain.Models;
using Moq;

namespace FlightBooking.Tests.Application;

public class FlightServiceTests
{
    private readonly Mock<IFlightRepository> _flightRepository = new();
    private readonly FlightService _service;
    
    public FlightServiceTests()
    {
        _service = new FlightService(_flightRepository.Object);
    }

    [Fact]
    public async Task GetAllFlightsAsync_ReturnsListOfFlights()
    {
        // Arrange
        var flights = new List<Flight>
        {
            CreateFlight(1, "1", "Kyiv", "London"),
            CreateFlight(2, "2", "Kyiv", "London")
        };
        _flightRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(flights);
        
        // Act
        var result = await _service.GetAllFlightsAsync();
        
        // Assert
        Assert.Equal(flights.Count, result.Count);
        Assert.Contains(flights, fl => result.Contains(fl));
    }
    
    [Fact]
    public async Task GetFlightByIdAsync_FlightExists_ReturnsFlight()
    {
        // Arrange
        var flight = CreateFlight(1, "1", "Kyiv", "London");
        _flightRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(flight);
        
        // Act
        var result = await _service.GetFlightByIdAsync(1);
        
        // Assert
        Assert.Equal(flight.Id, result.Id);
        Assert.Equal(flight.FlightNumber, result.FlightNumber);
    }
    
    [Fact]
    public async Task GetFlightByIdAsync_FlightNotFound_ThrowsFlightNotFoundException()
    {
        // Arrange
        _flightRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Flight?)null);
        
        // Act
        var act = async() => await _service.GetFlightByIdAsync(1);
        
        // Assert
        await Assert.ThrowsAsync<FlightNotFoundException>(act);
    }
    
    [Fact]
    public async Task AddFlightAsync_ValidInput_ReturnsCreatedFlight()
    {
        // Arrange
        var flight = CreateFlight(1, "1", "Kyiv", "London");
        _flightRepository.Setup(r => r.ExistsByNumberAsync(It.IsAny<string>())).ReturnsAsync(false);
        
        // Act
        var result = await _service.AddFlightAsync(flight);
        
        // Assert
        Assert.Equal(flight.Id, result.Id);
        Assert.Equal(flight.FlightNumber, result.FlightNumber);
        
        _flightRepository.Verify(r => r.AddAsync(flight), Times.Once);
    }
    
    [Fact]
    public async Task AddFlightAsync_AlreadyExists_ThrowsFlightAlreadyExistsException()
    {
        // Arrange
        var flight = CreateFlight(1, "1", "Kyiv", "London");
        _flightRepository.Setup(r => r.ExistsByNumberAsync(It.IsAny<string>())).ReturnsAsync(true);
        
        // Act
        var act = async() => await _service.AddFlightAsync(flight);
        
        // Assert
        await Assert.ThrowsAsync<FlightAlreadyExistsException>(act);
    }
    
    [Fact]
    public async Task UpdateFlightAsync_ValidInput_ReturnsUpdatedFlight()
    {
        // Arrange
        var flight = CreateFlight(1, "1", "Kyiv", "London");
        var dto = CreateUpdateFlightRequestDto("1V", "London", "Warsaw");
        
        _flightRepository.Setup(r => r.ExistsByIdAsync(1)).ReturnsAsync(true);
        _flightRepository.Setup(r => r.ExistsByNumberAsync("1V")).ReturnsAsync(false);
        _flightRepository.Setup(r => r.UpdateAsync(1, It.IsAny<Flight>())).Returns(Task.CompletedTask);

        // Act
        var updatedFlight = await _service.UpdateFlightAsync(1, dto);

        // Assert
        Assert.NotNull(updatedFlight);
        Assert.Equal(1, updatedFlight.Id);
        Assert.Equal("1V", updatedFlight.FlightNumber);
        Assert.Equal("London", updatedFlight.From);
        Assert.Equal("Warsaw", updatedFlight.To);

        _flightRepository.Verify(r => r.UpdateAsync(1, It.IsAny<Flight>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateFlightAsync_FlightNotFound_ThrowsFlightNotFoundException()
    {
        // Arrange
        var dto = CreateUpdateFlightRequestDto("1V", "London", "Warsaw");
        _flightRepository.Setup(r => r.ExistsByIdAsync(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        var act = async () => await _service.UpdateFlightAsync(1, dto);

        // Assert
        await Assert.ThrowsAsync<FlightNotFoundException>(act);
    }

    [Fact]
    public async Task UpdateFlightAsync_FlightNumberExistsOnAnotherFlight_ThrowsFlightAlreadyExistsException()
    {
        // Arrange
        var dto = CreateUpdateFlightRequestDto("1V", "London", "Warsaw");
    
        _flightRepository.Setup(r => r.ExistsByIdAsync(1)).ReturnsAsync(true);
    
        var flights = new List<Flight>
        {
            new() { Id = 1, FlightNumber = "1A" }, 
            new() { Id = 2, FlightNumber = "1V" } 
        };
    
        _flightRepository.Setup(r => r.ExistsByNumberAsync("1V")).ReturnsAsync(true);
        _flightRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(flights);

        // Act
        var act = async () => await _service.UpdateFlightAsync(1, dto);

        // Assert
        await Assert.ThrowsAsync<FlightAlreadyExistsException>(act);
    }
    
    [Fact]
    public async Task DeleteFlightAsync_ExistingId_CallsRepositoryDeleteOnce()
    {
        // Arrange
        int flightId = 1;
        _flightRepository.Setup(r => r.DeleteAsync(flightId)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteFlightAsync(flightId);

        // Assert
        _flightRepository.Verify(r => r.DeleteAsync(flightId), Times.Once);
    }

    [Fact]
    public async Task DeleteFlightAsync_NonExistingId_DoesNotThrow()
    {
        // Arrange
        int flightId = 99;
        _flightRepository.Setup(r => r.DeleteAsync(flightId)).Returns(Task.CompletedTask);

        // Act 
        var exception = await Record.ExceptionAsync(() => _service.DeleteFlightAsync(flightId));
        
        // Assert
        Assert.Null(exception); 
    }

    [Fact]
    public async Task DeleteFlightAsync_CalledMultipleTimes_IsIdempotent()
    {
        // Arrange
        int flightId = 42;
        _flightRepository.Setup(r => r.DeleteAsync(flightId)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteFlightAsync(flightId);
        await _service.DeleteFlightAsync(flightId);

        // Assert
        _flightRepository.Verify(r => r.DeleteAsync(flightId), Times.Exactly(2));
    }
    
    [Theory]
    [InlineData("kyiv", null, 2)] 
    [InlineData(null, "Rome", 2)] 
    [InlineData("Kyiv", "rome", 1)] 
    [InlineData(null, null, 3)] 
    public async Task GetFlightsAsync_Filtering_ReturnsExpectedCount(
        string from, string to, int expectedCount)
    {
        // Arrange
        var flights = new List<Flight>
        {
            CreateFlight(1, "1", "Kyiv", "Rome"),
            CreateFlight(2, "2", "Paris", "Rome"),
            CreateFlight(3, "3", "Kyiv", "London")
        };

        _flightRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(flights);

        var query = new FlightQueryDto
        {
            From = from,
            To = to
        };

        // Act
        var result = await _service.GetFlightsAsync(query);

        // Assert
        Assert.Equal(expectedCount, result.Count);
    }

    private static Flight CreateFlight(int flightId, string flightNumber, string from, string to)
    {
        return new Flight
        {
            Id = flightId,
            FlightNumber = flightNumber,
            From = from,
            To = to,
            Seats = new List<Seat>
            {
                new() { Id = 1, SeatNumber = "1" }
            }
        };
    }
    
    private static UpdateFlightRequestDto CreateUpdateFlightRequestDto(string flightNumber, string from, string to)
    {
        return new UpdateFlightRequestDto
        {
            FlightNumber = flightNumber,
            From = from,
            To = to
        };
    }
}