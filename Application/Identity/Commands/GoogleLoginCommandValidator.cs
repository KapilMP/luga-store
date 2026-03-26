using FluentValidation;

namespace LugaStore.Application.Identity.Commands;

public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        RuleFor(v => v.IdToken).NotEmpty().WithMessage("Google ID Token is required.");
    }
}
