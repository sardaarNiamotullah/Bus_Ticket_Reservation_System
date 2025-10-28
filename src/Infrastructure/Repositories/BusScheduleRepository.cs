using Application.Contracts.Interfaces.Repositories;
using Domain.Entities.ScheduleAggregate;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BusScheduleRepository : Repository<BusSchedule>, IBusScheduleRepository
{
    public BusScheduleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<BusSchedule?> GetByIdWithSeatsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Seats)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<BusSchedule>> GetSchedulesByDateAsync(
        DateOnly journeyDate, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Seats)
            .Where(s => s.JourneyDate == journeyDate 
                     && s.Status == ScheduleStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BusSchedule>> SearchSchedulesAsync(
        string fromCity, 
        string toCity, 
        DateOnly journeyDate, 
        CancellationToken cancellationToken = default)
    {
        // This requires joining with Routes - we'll do this in the query
        var schedules = await _context.BusSchedules
            .Include(s => s.Seats)
            .Where(s => s.JourneyDate == journeyDate 
                     && s.Status == ScheduleStatus.Active)
            .ToListAsync(cancellationToken);

        // Filter by route cities (we'll need RouteId to match)
        var routeIds = await _context.Routes
            .Where(r => r.FromCity.ToLower() == fromCity.ToLower() 
                     && r.ToCity.ToLower() == toCity.ToLower())
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        return schedules.Where(s => routeIds.Contains(s.RouteId));
    }
}