using Domain.Enums;

namespace Domain.Entities.ScheduleAggregate;

public class BusSchedule : BaseEntity
{
    public Guid BusId { get; private set; }
    public Guid RouteId { get; private set; }
    public DateOnly JourneyDate { get; private set; }
    public int AvailableSeats { get; private set; }
    public ScheduleStatus Status { get; private set; }

    // Navigation properties
    private readonly List<Seat> _seats = new();
    public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();

    private BusSchedule() { } // For EF Core

    public BusSchedule(Guid busId, Guid routeId, DateOnly journeyDate, int totalSeats)
    {
        if (busId == Guid.Empty)
            throw new ArgumentException("Bus ID cannot be empty", nameof(busId));

        if (routeId == Guid.Empty)
            throw new ArgumentException("Route ID cannot be empty", nameof(routeId));

        if (totalSeats <= 0)
            throw new ArgumentException("Total seats must be greater than zero", nameof(totalSeats));

        BusId = busId;
        RouteId = routeId;
        JourneyDate = journeyDate;
        AvailableSeats = totalSeats;
        Status = ScheduleStatus.Active;

        // Initialize seats
        InitializeSeats(totalSeats);
    }

    private void InitializeSeats(int totalSeats)
    {
        for (int i = 1; i <= totalSeats; i++)
        {
            _seats.Add(new Seat(Id, i));
        }
    }

    public void DecrementAvailableSeats()
    {
        if (AvailableSeats <= 0)
            throw new InvalidOperationException("No available seats to decrement");

        AvailableSeats--;
        UpdateTimestamp();
    }

    public void IncrementAvailableSeats()
    {
        AvailableSeats++;
        UpdateTimestamp();
    }

    public void CancelSchedule()
    {
        Status = ScheduleStatus.Cancelled;
        UpdateTimestamp();
    }

    public void CompleteSchedule()
    {
        Status = ScheduleStatus.Completed;
        UpdateTimestamp();
    }

    public bool IsActive() => Status == ScheduleStatus.Active;
}