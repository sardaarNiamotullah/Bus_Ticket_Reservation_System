namespace Domain.Exceptions;

public class SeatNotAvailableException : DomainException
{
    public SeatNotAvailableException(int seatNumber) 
        : base($"Seat {seatNumber} is not available for booking")
    {
    }
}