using Application.Contracts.DTOs.Search;

namespace Application.Contracts.Interfaces.Services;

public interface ISearchService
{
    Task<IEnumerable<AvailableBusDto>> SearchAvailableBusesAsync(
        SearchBusInputDto input, 
        CancellationToken cancellationToken = default);
}