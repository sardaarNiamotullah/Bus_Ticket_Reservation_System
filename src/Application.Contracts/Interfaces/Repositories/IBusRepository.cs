using Domain.Entities.BusAggregate;

namespace Application.Contracts.Interfaces.Repositories;

public interface IBusRepository : IRepository<Bus>
{
    Task<Bus?> GetByBusNumberAsync(string busNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bus>> GetActiveBusesAsync(CancellationToken cancellationToken = default);
}