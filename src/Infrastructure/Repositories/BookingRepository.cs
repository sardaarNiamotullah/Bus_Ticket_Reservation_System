using Application.Contracts.Interfaces.Repositories;
using Domain.Entities.BookingAggregate;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Booking>> GetBookingsByScheduleAsync(
        Guid scheduleId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(b => b.ScheduleId == scheduleId 
                     && b.Status == BookingStatus.Confirmed)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(b => b.PassengerEmail.ToLower() == email.ToLower())
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Booking?> GetBookingByScheduleAndSeatAsync(
        Guid scheduleId, 
        int seatNumber, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                b => b.ScheduleId == scheduleId 
                  && b.SeatNumber == seatNumber 
                  && b.Status == BookingStatus.Confirmed, 
                cancellationToken);
    }
}