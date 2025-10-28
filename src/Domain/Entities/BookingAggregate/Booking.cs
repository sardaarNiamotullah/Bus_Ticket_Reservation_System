using Domain.Enums;

namespace Domain.Entities.BookingAggregate;

public class Booking : BaseEntity
{
    public Guid ScheduleId { get; private set; }
    public int SeatNumber { get; private set; }
    public string PassengerName { get; private set; }
    public string PassengerEmail { get; private set; }
    public BookingType BookingType { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime BookingDate { get; private set; }

    private Booking(){
        PassengerName = null!;
        PassengerEmail = null!;
    } // For EF Core

    public Booking(
        Guid scheduleId,
        int seatNumber,
        string passengerName,
        string passengerEmail,
        BookingType bookingType)
    {
        if (scheduleId == Guid.Empty)
            throw new ArgumentException("Schedule ID cannot be empty", nameof(scheduleId));

        if (seatNumber <= 0)
            throw new ArgumentException("Seat number must be greater than zero", nameof(seatNumber));

        if (string.IsNullOrWhiteSpace(passengerName))
            throw new ArgumentException("Passenger name cannot be empty", nameof(passengerName));

        if (string.IsNullOrWhiteSpace(passengerEmail))
            throw new ArgumentException("Passenger email cannot be empty", nameof(passengerEmail));

        ScheduleId = scheduleId;
        SeatNumber = seatNumber;
        PassengerName = passengerName;
        PassengerEmail = passengerEmail;
        BookingType = bookingType;
        Status = BookingStatus.Confirmed;
        BookingDate = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be cancelled");

        Status = BookingStatus.Cancelled;
        UpdateTimestamp();
    }

    public bool IsConfirmed() => Status == BookingStatus.Confirmed;
}