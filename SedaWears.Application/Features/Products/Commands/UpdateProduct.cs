using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Application.Features.Products.Models;

namespace SedaWears.Application.Features.Products.Commands;

public record UpdateProductCommand(int Id, string Name, string? Description, decimal Price, int CategoryId, List<ProductSizeRepresentation> Sizes) : IRequest<Unit>;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid product identifier is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category identifier is required.");

        RuleFor(x => x.Sizes)
            .NotEmpty().WithMessage("At least one size with stock must be provided.");
    }
}

public class UpdateProductHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await dbContext.Products
            .Include(p => p.SizeStocks)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct) ?? throw new NotFoundException("Product not found");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        // Sync sizes
        product.SizeStocks.Clear();
        foreach (var s in request.Sizes)
        {
            product.SizeStocks.Add(new ProductSizeStock { Size = s.Size, Stock = s.Stock });
        }

        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
