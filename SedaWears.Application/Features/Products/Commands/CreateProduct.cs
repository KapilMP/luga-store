using MediatR;
using FluentValidation;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using SedaWears.Application.Features.Products.Models;

namespace SedaWears.Application.Features.Products.Commands;

public record CreateProductCommand(string Name, string? Description, decimal Price, decimal ShippingCost, Gender Gender, bool IsFeatured, bool IsNew, int CategoryId, List<ProductSizeRepresentation> Sizes) : IRequest<int>;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
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

public class CreateProductHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ShippingCost = request.ShippingCost,
            Gender = request.Gender,
            IsFeatured = request.IsFeatured,
            IsNew = request.IsNew,
            CategoryId = request.CategoryId,
            SizeStocks = request.Sizes.Select(s => new ProductSizeStock { Size = s.Size, Stock = s.Stock }).ToList()
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);
        return product.Id;
    }
}
