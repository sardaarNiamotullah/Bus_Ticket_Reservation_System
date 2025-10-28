using Application.Contracts.DTOs.Booking;
using Application.Contracts.Interfaces.Repositories;
using Application.Contracts.Interfaces.Services;
using AutoMapper;
using Domain.Entities.BookingAggregate;
using Domain.Enums;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISeatBookingDomainService _seatBookingDomainService;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ISeatBookingDomainService seatBookingDomainService,
        ILogger<BookingService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _seatBookingDomainService = seatBookingDomainService;
        _logger = logger;
    }

    public async Task<SeatPlanDto> GetSeatPlanAsync(
        Guid busScheduleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting seat plan for schedule {ScheduleId}", busScheduleId);

            var schedule = await _unitOfWork.BusSchedules.GetByIdWithSeatsAsync(
                busScheduleId,
                cancellationToken);

            if (schedule == null)
            {
                _logger.LogWarning("Schedule {ScheduleId} not found", busScheduleId);
                throw new KeyNotFoundException($"Schedule with ID {busScheduleId} not found");
            }

            // Get bus and route information
            var bus = await _unitOfWork.Buses.GetByIdAsync(schedule.BusId, cancellationToken);
            var route = await _unitOfWork.Routes.GetByIdAsync(schedule.RouteId, cancellationToken);

            if (bus == null || route == null)
            {
                throw new InvalidOperationException("Bus or Route information not found");
            }

            var seatDtos = _mapper.Map<List<SeatDto>>(schedule.Seats.OrderBy(s => s.SeatNumber));

            var seatPlan = new SeatPlanDto
            {
                ScheduleId = schedule.Id,
                BusName = bus.Name,
                Route = $"{route.FromCity} to {route.ToCity}",
                JourneyDate = schedule.JourneyDate,
                TotalSeats = bus.TotalSeats,
                AvailableSeats = schedule.AvailableSeats,
                Seats = seatDtos
            };

            _logger.LogInformation(
                "Seat plan retrieved: {Available}/{Total} seats available",
                schedule.AvailableSeats, bus.TotalSeats);

            return seatPlan;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seat plan for schedule {ScheduleId}", busScheduleId);
            throw;
        }
    }

    public async Task<BookingResultDto> BookSeatAsync(
        BookSeatInputDto input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Booking seats {Seats} for schedule {ScheduleId}",
                string.Join(",", input.SeatNumbers), input.ScheduleId);

            // Parse booking type
            if (!Enum.TryParse<BookingType>(input.BookingType, out var bookingType))
            {
                throw new ArgumentException($"Invalid booking type: {input.BookingType}");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Get schedule with seats
                var schedule = await _unitOfWork.BusSchedules.GetByIdWithSeatsAsync(
                    input.ScheduleId,
                    cancellationToken);

                if (schedule == null)
                {
                    throw new KeyNotFoundException($"Schedule with ID {input.ScheduleId} not found");
                }

                if (!schedule.IsActive())
                {
                    throw new InvalidOperationException("Schedule is not active");
                }

                // Validate and book each seat
                var bookingIds = new List<Guid>();
                var bookedSeats = new List<int>();

                foreach (var seatNumber in input.SeatNumbers)
                {
                    // Find the seat
                    var seat = schedule.Seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
                    if (seat == null)
                    {
                        throw new ArgumentException($"Seat {seatNumber} does not exist");
                    }

                    // Check if seat can be booked using domain service
                    if (!_seatBookingDomainService.CanBookSeat(seat))
                    {
                        throw new InvalidOperationException(
                            $"Seat {seatNumber} is not available for booking");
                    }

                    // Book the seat using domain service
                    _seatBookingDomainService.BookSeat(seat, bookingType);

                    // Create booking entity
                    var booking = new Booking(
                        input.ScheduleId,
                        seatNumber,
                        input.PassengerName,
                        input.PassengerEmail,
                        bookingType);

                    await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);

                    bookingIds.Add(booking.Id);
                    bookedSeats.Add(seatNumber);

                    // Update schedule available seats
                    schedule.DecrementAvailableSeats();
                }

                await _unitOfWork.BusSchedules.UpdateAsync(schedule, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var result = new BookingResultDto
                {
                    BookingIds = bookingIds,
                    ScheduleId = input.ScheduleId,
                    SeatNumbers = bookedSeats,
                    PassengerName = input.PassengerName,
                    PassengerEmail = input.PassengerEmail,
                    BookingType = input.BookingType,
                    BookingDate = DateTime.UtcNow,
                    Success = true,
                    Message = $"Successfully booked {bookedSeats.Count} seat(s)"
                };

                _logger.LogInformation(
                    "Successfully booked seats {Seats} for {Email}",
                    string.Join(",", bookedSeats), input.PassengerEmail);

                return result;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking seats");
            throw;
        }
    }

    public async Task<bool> CancelBookingAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Cancelling booking {BookingId}", bookingId);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId, cancellationToken);
                if (booking == null)
                {
                    _logger.LogWarning("Booking {BookingId} not found", bookingId);
                    return false;
                }

                if (!booking.IsConfirmed())
                {
                    _logger.LogWarning("Booking {BookingId} is not confirmed", bookingId);
                    return false;
                }

                // Get schedule with seats
                var schedule = await _unitOfWork.BusSchedules.GetByIdWithSeatsAsync(
                    booking.ScheduleId,
                    cancellationToken);

                if (schedule == null)
                {
                    throw new InvalidOperationException("Schedule not found");
                }

                // Find and release the seat
                var seat = schedule.Seats.FirstOrDefault(s => s.SeatNumber == booking.SeatNumber);
                if (seat != null)
                {
                    _seatBookingDomainService.ReleaseSeat(seat);
                    schedule.IncrementAvailableSeats();
                }

                // Cancel the booking
                booking.Cancel();

                await _unitOfWork.Bookings.UpdateAsync(booking, cancellationToken);
                await _unitOfWork.BusSchedules.UpdateAsync(schedule, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Booking {BookingId} cancelled successfully", bookingId);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);
            throw;
        }
    }
}