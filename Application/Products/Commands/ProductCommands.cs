using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Products.Commands;

public record DeleteProductCommand(int ProductId, int? RequestingUserId = null, bool IsAdmin = false) : IRequest<bool>;

public class DeleteProductCommandHandler(IProductService productService) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
        => await productService.DeleteProductAsync(request.ProductId, request.RequestingUserId, request.IsAdmin, ct);
}

public record ProductSizeStockDto(ProductSize Size, int Stock);
public record SetProductSizesCommand(int ProductId, List<ProductSizeStockDto> Sizes, int? RequestingUserId = null, bool IsAdmin = false) : IRequest<bool>;

public class SetProductSizesCommandHandler(IProductService productService) : IRequestHandler<SetProductSizesCommand, bool>
{
    public async Task<bool> Handle(SetProductSizesCommand request, CancellationToken ct)
        => await productService.SetProductSizesAsync(request.ProductId, request.Sizes, request.RequestingUserId, request.IsAdmin, ct);
}

public record CreatePartnerProductCommand(string Name, string? Description, decimal Price, Gender Gender, List<int> CategoryIds, int PartnerId) : IRequest<int>;

public class CreatePartnerProductCommandHandler(IProductService productService) : IRequestHandler<CreatePartnerProductCommand, int>
{
    public async Task<int> Handle(CreatePartnerProductCommand request, CancellationToken ct)
        => await productService.CreateProductAsync(request.Name, request.Description, request.Price, request.Gender, request.CategoryIds, request.PartnerId, ct);
}
