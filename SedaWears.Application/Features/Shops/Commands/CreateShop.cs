using MediatR;
using FluentValidation;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Application.Features.Shops.Models;

namespace SedaWears.Application.Features.Shops.Commands;

public record CreateShopCommand(string Name, string Slug, string? Description) : IRequest<int>;

public class CreateShopValidator : AbstractValidator<CreateShopCommand>
{
    public CreateShopValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200);
    }
}

public class CreateShopHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateShopCommand, int>
{
    public async Task<int> Handle(CreateShopCommand request, CancellationToken ct)
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

        return shop.Id;
    }
}
