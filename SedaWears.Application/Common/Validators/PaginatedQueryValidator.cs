using FluentValidation;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Common.Validators;

public class PaginatedQueryValidator<T> : AbstractValidator<T> where T : IPaginatedQuery
{
    public PaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be at least 1.")
            .LessThanOrEqualTo(30)
            .WithMessage("Page size cannot exceed 30.");
    }
}
