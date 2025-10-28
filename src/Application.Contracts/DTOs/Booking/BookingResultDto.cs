namespace Application.Contracts.DTOs.Booking;

public class BookingResultDto
{
    public List<Guid> BookingIds { get; set; } = new();
    public Guid ScheduleId { get; set; }
    public List<int> SeatNumbers { get; set; } = new();
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerEmail { get; set; } = string.Empty;
    public string BookingType { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}