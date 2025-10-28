namespace Application.Contracts.DTOs.Search;

public class SearchBusInputDto
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public DateTime JourneyDate { get; set; }
}