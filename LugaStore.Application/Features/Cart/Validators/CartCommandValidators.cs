using FluentValidation;
using LugaStore.Application.Features.Cart.Commands;

namespace LugaStore.Application.Features.Cart.Validators;

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
