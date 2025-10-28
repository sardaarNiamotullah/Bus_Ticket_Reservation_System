using Domain.ValueObjects;

namespace Domain.Entities.BusAggregate;

public class Bus : BaseEntity
{
    public string Name { get; private set; }
    public string BusNumber { get; private set; }
    public bool IsAC { get; private set; }
    public int TotalSeats { get; private set; }
    public Money FarePerSeat { get; private set; }

    private Bus()
    {
        Name = null!;
        BusNumber = null!;
        FarePerSeat = null!;
    } // For EF Core

    public Bus(string name, string busNumber, bool isAC, int totalSeats, Money farePerSeat)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Bus name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(busNumber))
            throw new ArgumentException("Bus number cannot be empty", nameof(busNumber));

        if (totalSeats <= 0)
            throw new ArgumentException("Total seats must be greater than zero", nameof(totalSeats));

        Name = name;
        BusNumber = busNumber;
        IsAC = isAC;
        TotalSeats = totalSeats;
        FarePerSeat = farePerSeat ?? throw new ArgumentNullException(nameof(farePerSeat));
    }

    public void UpdateDetails(string name, bool isAC, Money farePerSeat)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        IsAC = isAC;

        if (farePerSeat != null)
            FarePerSeat = farePerSeat;

        UpdateTimestamp();
    }
}