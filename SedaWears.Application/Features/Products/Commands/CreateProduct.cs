using MediatR;
using FluentValidation;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Products.Commands;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    Gender Gender,
    int CategoryId,
    List<string> ImageFileNames) : IRequest;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category identifier is required.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("A valid gender must be specified.");

        RuleFor(x => x.ImageFileNames)
            .NotEmpty().WithMessage("At least one image must be provided.");
    }
}

public class CreateProductHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateProductCommand>
{
    public async Task Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            Gender = request.Gender,
            Images = request.ImageFileNames.Select((fileName, index) => new ProductImage
            {
                FileName = fileName,
                Order = index
            }).ToList()
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);
    }
}
