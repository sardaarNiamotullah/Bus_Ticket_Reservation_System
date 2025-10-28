using Domain.Entities.ScheduleAggregate;
using Domain.Enums;

namespace Domain.Services;

public interface ISeatBookingDomainService
{
    void BookSeat(Seat seat, BookingType bookingType);
    void ReleaseSeat(Seat seat);
    bool CanBookSeat(Seat seat);
}