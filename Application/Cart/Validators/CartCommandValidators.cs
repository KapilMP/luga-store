using FluentValidation;
using LugaStore.Application.Cart.Commands;

namespace LugaStore.Application.Cart.Validators;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(v => v.ProductId).GreaterThan(0);
        RuleFor(v => v.Quantity).GreaterThan(0);
    }
}

public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(v => v.Quantity).GreaterThan(0);
    }
}
