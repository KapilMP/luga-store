using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Products.Commands;

public record CreateProductCommand(
    string Name, 
    string? Description, 
    decimal Price, 
    Gender Gender,
    List<int> CategoryIds) : IRequest<int>;

public class CreateProductCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Gender = request.Gender
        };

        if (request.CategoryIds.Count > 0)
        {
            var categories = await context.Categories
                .Where(c => request.CategoryIds.Contains(c.Id))
                .ToListAsync(ct);

            foreach (var category in categories)
                product.Categories.Add(category);
        }

        context.Products.Add(product);
        await context.SaveChangesAsync(ct);
        return product.Id;
    }
}
