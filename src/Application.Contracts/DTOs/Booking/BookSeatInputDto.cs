namespace Application.Contracts.DTOs.Booking;

public class BookSeatInputDto
{
    public Guid ScheduleId { get; set; }
    public List<int> SeatNumbers { get; set; } = new();
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerEmail { get; set; } = string.Empty;
    public string BookingType { get; set; } = "Book"; // "Book" or "Buy"
}