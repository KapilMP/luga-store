using FluentValidation;
using LugaStore.Application.Features.Products.Queries;

namespace LugaStore.Application.Features.Products.Validators;

public class GetMyProductsQueryValidator : AbstractValidator<GetMyProductsQuery>
{
    public GetMyProductsQueryValidator() => RuleFor(v => v.PartnerId).GreaterThan(0);
}

public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator() => RuleFor(v => v.ProductId).GreaterThan(0);
}
