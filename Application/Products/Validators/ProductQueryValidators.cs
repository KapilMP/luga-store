using FluentValidation;
using LugaStore.Application.Products.Queries;

namespace LugaStore.Application.Products.Validators;

public class GetMyProductsQueryValidator : AbstractValidator<GetMyProductsQuery>
{
    public GetMyProductsQueryValidator() => RuleFor(v => v.CreatorId).GreaterThan(0);
}

public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator() => RuleFor(v => v.ProductId).GreaterThan(0);
}
