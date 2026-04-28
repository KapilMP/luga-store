using MediatR;
using FluentValidation;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Products.Commands;

public record CreateProductSaleCommand(int ProductId, decimal DiscountedPrice, decimal? DiscountPercent, DateTime StartsAt, DateTime? EndsAt) : IRequest<int>;

public class CreateProductSaleValidator : AbstractValidator<CreateProductSaleCommand>
{
    public CreateProductSaleValidator()
    {
        RuleFor(x => x.DiscountedPrice).GreaterThan(0);
        RuleFor(x => x.StartsAt).NotEmpty();
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt).When(x => x.EndsAt.HasValue);
    }
}

public class CreateProductSaleHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateProductSaleCommand, int>
{
    public async Task<int> Handle(CreateProductSaleCommand request, CancellationToken ct)
    {
        _ = await dbContext.Products.FindAsync([request.ProductId], ct)
            ?? throw new NotFoundException("Product not found");

        var sale = new ProductSale
        {
            ProductId = request.ProductId,
            DiscountedPrice = request.DiscountedPrice,
            DiscountPercent = request.DiscountPercent,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            IsActive = true
        };

        dbContext.ProductSales.Add(sale);
        await dbContext.SaveChangesAsync(ct);
        return sale.Id;
    }
}
