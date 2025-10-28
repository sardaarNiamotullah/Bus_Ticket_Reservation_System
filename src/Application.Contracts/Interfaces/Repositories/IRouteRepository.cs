using Domain.Entities.RouteAggregate;

namespace Application.Contracts.Interfaces.Repositories;

public interface IRouteRepository : IRepository<Route>
{
    Task<IEnumerable<Route>> GetRoutesByCitiesAsync(
        string fromCity, 
        string toCity, 
        CancellationToken cancellationToken = default);
}