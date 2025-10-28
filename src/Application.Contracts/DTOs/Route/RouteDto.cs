namespace Application.Contracts.DTOs.Route;

public class RouteDto
{
    public Guid Id { get; set; }
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public int DurationMinutes { get; set; }
    
    // Formatted strings for display
    public string DepartureTimeFormatted => DepartureTime.ToString(@"hh\:mm");
    public string ArrivalTimeFormatted => ArrivalTime.ToString(@"hh\:mm");
}