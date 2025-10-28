using Application.Contracts.DTOs.Booking;

namespace Application.Contracts.Interfaces.Services;

public interface IBookingService
{
    Task<SeatPlanDto> GetSeatPlanAsync(
        Guid busScheduleId, 
        CancellationToken cancellationToken = default);
    
    Task<BookingResultDto> BookSeatAsync(
        BookSeatInputDto input, 
        CancellationToken cancellationToken = default);
    
    Task<bool> CancelBookingAsync(
        Guid bookingId, 
        CancellationToken cancellationToken = default);
}