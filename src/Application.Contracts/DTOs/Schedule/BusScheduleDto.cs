namespace Application.Contracts.DTOs.Schedule;

public class BusScheduleDto
{
    public Guid Id { get; set; }
    public Guid BusId { get; set; }
    public Guid RouteId { get; set; }
    public DateOnly JourneyDate { get; set; }
    public int AvailableSeats { get; set; }
    public string Status { get; set; } = string.Empty;
}