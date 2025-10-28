namespace Application.Contracts.DTOs.Bus;

public class BusDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BusNumber { get; set; } = string.Empty;
    public bool IsAC { get; set; }
    public int TotalSeats { get; set; }
    public decimal FarePerSeat { get; set; }
    public string Currency { get; set; } = "BDT";
}