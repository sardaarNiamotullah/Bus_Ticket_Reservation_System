using Domain.Entities.BookingAggregate;

namespace Application.Contracts.Interfaces.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetBookingsByScheduleAsync(
        Guid scheduleId, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Booking>> GetBookingsByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default);
    
    Task<Booking?> GetBookingByScheduleAndSeatAsync(
        Guid scheduleId, 
        int seatNumber, 
        CancellationToken cancellationToken = default);
}