using FluentValidation;
using SedaWears.Application.Features.Orders.Commands;

namespace SedaWears.Application.Features.Orders.Validators;

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
    }
}
