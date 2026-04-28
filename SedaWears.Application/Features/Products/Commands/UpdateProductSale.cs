using MediatR;
using FluentValidation;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Products.Commands;

public record UpdateProductSaleCommand(int ProductId, int SaleId, decimal DiscountedPrice, decimal? DiscountPercent, DateTime StartsAt, DateTime? EndsAt, bool IsActive) : IRequest<Unit>;

public class UpdateProductSaleValidator : AbstractValidator<UpdateProductSaleCommand>
{
    public UpdateProductSaleValidator()
    {
        RuleFor(x => x.DiscountedPrice).GreaterThan(0);
        RuleFor(x => x.StartsAt).NotEmpty();
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt).When(x => x.EndsAt.HasValue);
    }
}

public class UpdateProductSaleHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateProductSaleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductSaleCommand request, CancellationToken ct)
    {
        var sale = await dbContext.ProductSales.FindAsync([request.SaleId], ct)
            ?? throw new NotFoundException("Sale not found");

        if (sale.ProductId != request.ProductId)
            throw new NotFoundException("Sale not found");

        sale.DiscountedPrice = request.DiscountedPrice;
        sale.DiscountPercent = request.DiscountPercent;
        sale.StartsAt = request.StartsAt;
        sale.EndsAt = request.EndsAt;
        sale.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
