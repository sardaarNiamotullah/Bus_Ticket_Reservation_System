namespace Application.Contracts.DTOs.Booking;

public class SeatPlanDto
{
    public Guid ScheduleId { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateOnly JourneyDate { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public List<SeatDto> Seats { get; set; } = new();
}