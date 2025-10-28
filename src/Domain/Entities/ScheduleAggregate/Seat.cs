using Domain.Enums;

namespace Domain.Entities.ScheduleAggregate;

public class Seat : BaseEntity
{
    public Guid ScheduleId { get; private set; }
    public int SeatNumber { get; private set; }
    public SeatStatus Status { get; private set; }

    private Seat() { } // For EF Core

    public Seat(Guid scheduleId, int seatNumber)
    {
        if (scheduleId == Guid.Empty)
            throw new ArgumentException("Schedule ID cannot be empty", nameof(scheduleId));

        if (seatNumber <= 0)
            throw new ArgumentException("Seat number must be greater than zero", nameof(seatNumber));

        ScheduleId = scheduleId;
        SeatNumber = seatNumber;
        Status = SeatStatus.Available;
    }

    public bool IsAvailable() => Status == SeatStatus.Available;

    public void MarkAsBooked()
    {
        if (Status != SeatStatus.Available)
            throw new InvalidOperationException($"Seat {SeatNumber} is not available");

        Status = SeatStatus.Booked;
        UpdateTimestamp();
    }

    public void MarkAsSold()
    {
        if (Status != SeatStatus.Available)
            throw new InvalidOperationException($"Seat {SeatNumber} is not available");

        Status = SeatStatus.Sold;
        UpdateTimestamp();
    }

    public void Release()
    {
        if (Status == SeatStatus.Available)
            throw new InvalidOperationException($"Seat {SeatNumber} is already available");

        Status = SeatStatus.Available;
        UpdateTimestamp();
    }
}