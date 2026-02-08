using FlightBooking.Application.Dtos.Flight;
using FlightBooking.Application.Dtos.Seat;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Mappers;
using FlightBooking.Controllers;
using FlightBooking.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlightBooking.Tests.Controllers;

public class FlightControllerTests
{
    private readonly Mock<IFlightService> _flightService = new();
    private readonly FlightController _flightController;

    public FlightControllerTests()
    {
        _flightController = new FlightController(_flightService.Object);
    }

    [Fact]
    public async Task GetAll_NoQuery_ReturnsAllFlights()
    {
        // Arrange
        var flights = new List<Flight>
        {
            CreateFlight( 1, "F1", "Kyiv", "London" ),
            CreateFlight( 2, "F2", "Paris",  "Rome")
        };
        _flightService.Setup(s => s.GetAllFlightsAsync()).ReturnsAsync(flights);

        // Act
        var result = await _flightController.GetAll(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedFlights = Assert.IsAssignableFrom<IEnumerable<FlightDto>>(okResult.Value);
        Assert.Equal(2, returnedFlights.Count());
    }

    [Fact]
    public async Task GetById_ExistingFlight_ReturnsFlightDetails()
    {
        // Arrange
        var flight = CreateFlight( 1, "F1", "Kyiv", "London" );
        _flightService.Setup(s => s.GetFlightByIdAsync(1)).ReturnsAsync(flight);

        // Act
        var result = await _flightController.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<FlightDetailsDto>(okResult.Value);
        Assert.Equal(1, dto.Id);
        Assert.Equal("F1", dto.FlightNumber);
    }

    [Fact]
    public async Task CreateFlight_ValidInput_ReturnsCreatedFlight()
    {
        // Arrange
        var requestDto = new CreateFlightRequestDto
        {
            FlightNumber = "F1",
            From = "Kyiv",
            To = "London",
            Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2),
            Seats = new List<SeatRequestDto>
            {
                new() { SeatNumber = "1", Price = 100 }
            }
        };
    
        var flight = FlightMapper.ToFlight(requestDto);
        flight.Id = 1;

        _flightService.Setup(s => s.AddFlightAsync(It.IsAny<Flight>())).ReturnsAsync(flight);

        // Act
        var result = await _flightController.CreateFlight(requestDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<FlightDetailsDto>(createdResult.Value);

        Assert.Equal(1, dto.Id);                   
        Assert.Equal("F1", dto.FlightNumber);
    }

    [Fact]
    public async Task DeleteFlight_ExistingFlight_ReturnsNoContent()
    {
        // Arrange
        _flightService.Setup(s => s.DeleteFlightAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _flightController.DeleteFlight(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _flightService.Verify(s => s.DeleteFlightAsync(1), Times.Once);
    }

    private static Flight CreateFlight(int id, string flightNumber, string from, string to)
    {
        return new Flight
        {
            Id = id, 
            FlightNumber = flightNumber, 
            From = from, 
            To = to
        };
    }
}