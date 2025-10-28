using Application.Contracts.DTOs.Search;
using Application.Contracts.Interfaces.Repositories;
using Application.Contracts.Interfaces.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SearchService> _logger;

    public SearchService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<SearchService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AvailableBusDto>> SearchAvailableBusesAsync(
        SearchBusInputDto input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Searching buses from {From} to {To} on {Date}",
                input.From, input.To, input.JourneyDate);

            // Convert DateTime to DateOnly
            var journeyDate = DateOnly.FromDateTime(input.JourneyDate);

            // Get matching routes
            var routes = await _unitOfWork.Routes.GetRoutesByCitiesAsync(
                input.From,
                input.To,
                cancellationToken);

            if (!routes.Any())
            {
                _logger.LogInformation("No routes found from {From} to {To}", input.From, input.To);
                return Enumerable.Empty<AvailableBusDto>();
            }

            var routeIds = routes.Select(r => r.Id).ToList();

            // Get all schedules for the journey date
            var allSchedules = await _unitOfWork.BusSchedules.GetSchedulesByDateAsync(
                journeyDate,
                cancellationToken);

            // Filter schedules by matching routes
            var matchingSchedules = allSchedules
                .Where(s => routeIds.Contains(s.RouteId))
                .ToList();

            if (!matchingSchedules.Any())
            {
                _logger.LogInformation(
                    "No schedules found for routes on {Date}",
                    journeyDate);
                return Enumerable.Empty<AvailableBusDto>();
            }

            // Get all buses
            var busIds = matchingSchedules.Select(s => s.BusId).Distinct().ToList();
            var allBuses = await _unitOfWork.Buses.GetAllAsync(cancellationToken);
            var buses = allBuses.Where(b => busIds.Contains(b.Id)).ToList();

            // Build result DTOs
            var result = new List<AvailableBusDto>();

            foreach (var schedule in matchingSchedules)
            {
                var bus = buses.FirstOrDefault(b => b.Id == schedule.BusId);
                var route = routes.FirstOrDefault(r => r.Id == schedule.RouteId);

                if (bus == null || route == null)
                    continue;

                var availableBusDto = new AvailableBusDto
                {
                    Id = schedule.Id, // This is the Schedule ID
                    BusId = bus.Id,
                    Name = bus.Name,
                    From = route.FromCity,
                    To = route.ToCity,
                    DepartureTime = route.DepartureTime.ToString(@"hh\:mm"),
                    ArrivalTime = route.ArrivalTime.ToString(@"hh\:mm"),
                    Fare = bus.FarePerSeat.Amount,
                    SeatsLeft = schedule.AvailableSeats,
                    IsAC = bus.IsAC,
                    TotalSeats = bus.TotalSeats,
                    JourneyDate = schedule.JourneyDate
                };

                result.Add(availableBusDto);
            }

            _logger.LogInformation("Found {Count} available buses", result.Count);

            return result.OrderBy(b => b.DepartureTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching buses");
            throw;
        }
    }
}