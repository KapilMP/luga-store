using FluentValidation;
using LugaStore.Application.Orders.Commands;

namespace LugaStore.Application.Orders.Validators;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(v => v.Items).NotEmpty().WithMessage("Order must contain at least one item.");
        RuleForEach(v => v.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });

        When(v => !v.UserId.HasValue, () =>
        {
            RuleFor(v => v.CustomerEmail).NotEmpty().EmailAddress().WithMessage("Guest checkout requires a valid email.");
        });
    }
}
