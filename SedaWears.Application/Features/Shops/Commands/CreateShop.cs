using MediatR;
using FluentValidation;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Shops.Commands;

public record CreateShopCommand(string Name, string Slug, string? Description) : IRequest;

public class CreateShopValidator : AbstractValidator<CreateShopCommand>
{
    public CreateShopValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Shop name is required.")
            .MaximumLength(100).WithMessage("Shop name must not exceed 100 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Description must not exceed 300 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class CreateShopHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateShopCommand>
{
    public async Task Handle(CreateShopCommand request, CancellationToken ct)
    {
        var shop = new Shop
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            IsActive = true
        };

        dbContext.Shops.Add(shop);
        await dbContext.SaveChangesAsync(ct);
    }
}
