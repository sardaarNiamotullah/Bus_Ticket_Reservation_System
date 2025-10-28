using Application.Contracts.Interfaces.Repositories;
using Domain.Entities.RouteAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RouteRepository : Repository<Route>, IRouteRepository
{
    public RouteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Route>> GetRoutesByCitiesAsync(
        string fromCity, 
        string toCity, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.FromCity.ToLower() == fromCity.ToLower() 
                     && r.ToCity.ToLower() == toCity.ToLower())
            .ToListAsync(cancellationToken);
    }
}