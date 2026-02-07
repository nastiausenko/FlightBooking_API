using FlightBooking.Application.Exceptions;
using FlightBooking.Application.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.ExceptionHandlers;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception, "Exception occurred: {Message}", exception.Message);
        
        var (status, title) = exception switch
        {
            SeatNotFoundException => (404, "Seat not found"),
            BookingNotFoundException => (404, "Booking not found"),
            FlightNotFoundException => (404, "Flight not found"),
            BookingAlreadyCanceledException => (409, "Booking already canceled"),//TODO
            NoActiveBookingsException => (404, "No active bookings"),
            SeatNotAvailableException => (404, "Seat not available"),
            
            ForbiddenException => (403, "Forbidden"),
            UnauthorizedAccessException => (401, "Unauthorized"),

            _ => (500, "Server error")
        };


        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}