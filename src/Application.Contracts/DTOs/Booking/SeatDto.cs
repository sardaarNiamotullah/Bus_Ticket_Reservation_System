namespace Application.Contracts.DTOs.Booking;

public class SeatDto
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public int Row { get; set; }
    public string Status { get; set; } = string.Empty; // Available, Booked, Sold
    public bool IsBooked { get; set; }
    public bool IsSold { get; set; }
}