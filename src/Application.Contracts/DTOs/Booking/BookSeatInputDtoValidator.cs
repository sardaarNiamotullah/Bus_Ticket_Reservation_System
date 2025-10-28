using FluentValidation;

namespace Application.Contracts.DTOs.Booking;

public class BookSeatInputDtoValidator : AbstractValidator<BookSeatInputDto>
{
    public BookSeatInputDtoValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Schedule ID is required");

        RuleFor(x => x.SeatNumbers)
            .NotEmpty().WithMessage("At least one seat must be selected")
            .Must(seats => seats.All(s => s > 0)).WithMessage("Invalid seat numbers");

        RuleFor(x => x.PassengerName)
            .NotEmpty().WithMessage("Passenger name is required")
            .MaximumLength(100).WithMessage("Passenger name cannot exceed 100 characters");

        RuleFor(x => x.PassengerEmail)
            .NotEmpty().WithMessage("Passenger email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.BookingType)
            .NotEmpty().WithMessage("Booking type is required")
            .Must(type => type == "Book" || type == "Buy")
            .WithMessage("Booking type must be either 'Book' or 'Buy'");
    }
}