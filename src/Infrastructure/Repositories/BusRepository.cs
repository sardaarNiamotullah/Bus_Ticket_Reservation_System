using Application.Contracts.Interfaces.Repositories;
using Domain.Entities.BusAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BusRepository : Repository<Bus>, IBusRepository
{
    public BusRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Bus?> GetByBusNumberAsync(
        string busNumber, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(b => b.BusNumber == busNumber, cancellationToken);
    }

    public async Task<IEnumerable<Bus>> GetActiveBusesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(b => b.TotalSeats > 0)
            .ToListAsync(cancellationToken);
    }
}