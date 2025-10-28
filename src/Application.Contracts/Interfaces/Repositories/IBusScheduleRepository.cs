using Domain.Entities.ScheduleAggregate;

namespace Application.Contracts.Interfaces.Repositories;

public interface IBusScheduleRepository : IRepository<BusSchedule>
{
    Task<BusSchedule?> GetByIdWithSeatsAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<BusSchedule>> GetSchedulesByDateAsync(
        DateOnly journeyDate, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<BusSchedule>> SearchSchedulesAsync(
        string fromCity, 
        string toCity, 
        DateOnly journeyDate, 
        CancellationToken cancellationToken = default);
}