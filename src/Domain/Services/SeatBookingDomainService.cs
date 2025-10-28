using Domain.Entities.ScheduleAggregate;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Services;

public class SeatBookingDomainService : ISeatBookingDomainService
{
    public void BookSeat(Seat seat, BookingType bookingType)
    {
        if (!CanBookSeat(seat))
            throw new SeatNotAvailableException(seat.SeatNumber);

        switch (bookingType)
        {
            case BookingType.Book:
                seat.MarkAsBooked();
                break;
            case BookingType.Buy:
                seat.MarkAsSold();
                break;
            default:
                throw new ArgumentException("Invalid booking type", nameof(bookingType));
        }
    }

    public void ReleaseSeat(Seat seat)
    {
        seat.Release();
    }

    public bool CanBookSeat(Seat seat)
    {
        return seat.IsAvailable();
    }
}