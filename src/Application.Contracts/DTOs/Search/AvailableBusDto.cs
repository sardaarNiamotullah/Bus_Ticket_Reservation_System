namespace Application.Contracts.DTOs.Search;

public class AvailableBusDto
{
    public Guid Id { get; set; } // Schedule ID
    public Guid BusId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string DepartureTime { get; set; } = string.Empty;
    public string ArrivalTime { get; set; } = string.Empty;
    public decimal Fare { get; set; }
    public int SeatsLeft { get; set; }
    public bool IsAC { get; set; }
    public int TotalSeats { get; set; }
    public DateOnly JourneyDate { get; set; }
}