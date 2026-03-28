using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Products.Commands;

public record CreateProductCommand(
    string Name, 
    string? Description, 
    decimal Price, 
    Gender Gender,
    List<int> CategoryIds) : IRequest<int>;

public class CreateProductCommandHandler(IProductService productService) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
        => await productService.CreateProductAsync(request.Name, request.Description, request.Price, request.Gender, request.CategoryIds, ct: ct);
}
