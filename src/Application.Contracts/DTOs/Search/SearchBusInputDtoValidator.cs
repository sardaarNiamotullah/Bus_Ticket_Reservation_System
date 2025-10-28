using FluentValidation;

namespace Application.Contracts.DTOs.Search;

public class SearchBusInputDtoValidator : AbstractValidator<SearchBusInputDto>
{
    public SearchBusInputDtoValidator()
    {
        RuleFor(x => x.From)
            .NotEmpty().WithMessage("From city is required")
            .MaximumLength(100).WithMessage("From city cannot exceed 100 characters");

        RuleFor(x => x.To)
            .NotEmpty().WithMessage("To city is required")
            .MaximumLength(100).WithMessage("To city cannot exceed 100 characters")
            .NotEqual(x => x.From).WithMessage("From and To cities cannot be the same");

        RuleFor(x => x.JourneyDate)
            .NotEmpty().WithMessage("Journey date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Journey date cannot be in the past");
    }
}