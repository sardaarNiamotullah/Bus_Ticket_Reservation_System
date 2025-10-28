using Application.Contracts.DTOs.Booking;
using Application.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingService bookingService,
        ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Get seat plan for a bus schedule
    /// </summary>
    /// <param name="scheduleId">Bus schedule ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Seat plan with availability</returns>
    [HttpGet("seat-plan/{scheduleId}")]
    [ProducesResponseType(typeof(SeatPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SeatPlanDto>> GetSeatPlan(
        Guid scheduleId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _bookingService.GetSeatPlanAsync(scheduleId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Schedule not found: {ScheduleId}", scheduleId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seat plan");
            return StatusCode(500, new { message = "An error occurred while getting seat plan" });
        }
    }

    /// <summary>
    /// Book seats for a bus schedule
    /// </summary>
    /// <param name="input">Booking details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Booking confirmation</returns>
    [HttpPost("book")]
    [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResultDto>> BookSeat(
        [FromBody] BookSeatInputDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _bookingService.BookSeatAsync(input, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking seat");
            return StatusCode(500, new { message = "An error occurred while booking seat" });
        }
    }

    /// <summary>
    /// Cancel a booking
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cancellation result</returns>
    [HttpPost("cancel/{bookingId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CancelBooking(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _bookingService.CancelBookingAsync(bookingId, cancellationToken);
            
            if (!result)
            {
                return NotFound(new { message = "Booking not found or cannot be cancelled" });
            }

            return Ok(new { message = "Booking cancelled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking");
            return StatusCode(500, new { message = "An error occurred while cancelling booking" });
        }
    }
}