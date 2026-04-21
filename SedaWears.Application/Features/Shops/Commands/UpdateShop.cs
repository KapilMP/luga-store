using MediatR;
using FluentValidation;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Shops.Commands;

public record UpdateShopCommand(int Id, string Name, string Slug, string? Description, bool? IsActive) : IRequest;

public class UpdateShopValidator : AbstractValidator<UpdateShopCommand>
{
    public UpdateShopValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200);
    }
}

public class UpdateShopHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<UpdateShopCommand>
{
    public async Task Handle(UpdateShopCommand request, CancellationToken ct)
    {
        var shop = await dbContext.Shops
            .Include(s => s.Owners)
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct) ?? throw new NotFoundException("Shop not found");

        // Authorization check
        if (currentUser.Role != UserRole.Admin)
        {
            var isOwner = shop.Owners.Any(o => o.OwnerId == currentUser.Id);
            if (!isOwner) throw new NotFoundException("Shop not found.");
        }

        shop.Name = request.Name;
        shop.Slug = request.Slug;
        shop.Description = request.Description;

        if (request.IsActive.HasValue && currentUser.Role == UserRole.Admin)
        {
            shop.IsActive = request.IsActive.Value;
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
