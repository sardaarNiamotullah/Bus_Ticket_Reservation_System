using Application.Contracts.DTOs.Search;
using Application.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        ISearchService searchService,
        ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Search for available buses
    /// </summary>
    /// <param name="input">Search parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available buses</returns>
    [HttpPost("buses")]
    [ProducesResponseType(typeof(IEnumerable<AvailableBusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AvailableBusDto>>> SearchBuses(
        [FromBody] SearchBusInputDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _searchService.SearchAvailableBusesAsync(input, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching buses");
            return StatusCode(500, new { message = "An error occurred while searching buses" });
        }
    }

    /// <summary>
    /// Search buses using query parameters (alternative endpoint)
    /// </summary>
    [HttpGet("buses")]
    [ProducesResponseType(typeof(IEnumerable<AvailableBusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AvailableBusDto>>> SearchBusesQuery(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] DateTime journeyDate,
        CancellationToken cancellationToken)
    {
        try
        {
            var input = new SearchBusInputDto
            {
                From = from,
                To = to,
                JourneyDate = journeyDate
            };

            var result = await _searchService.SearchAvailableBusesAsync(input, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching buses");
            return StatusCode(500, new { message = "An error occurred while searching buses" });
        }
    }
}