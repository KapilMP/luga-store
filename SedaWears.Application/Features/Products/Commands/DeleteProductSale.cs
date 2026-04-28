using MediatR;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Products.Commands;

public record DeleteProductSaleCommand(int ProductId, int SaleId) : IRequest<Unit>;

public class DeleteProductSaleHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteProductSaleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductSaleCommand request, CancellationToken ct)
    {
        var sale = await dbContext.ProductSales.FindAsync([request.SaleId], ct)
            ?? throw new NotFoundException("Sale not found");

        if (sale.ProductId != request.ProductId)
            throw new NotFoundException("Sale not found");

        dbContext.ProductSales.Remove(sale);
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
