using MediatR;
using FluentValidation;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Shops.Commands;

public record UpdateShopCommand(
    int Id,
    string Name,
    string Slug,
    string? Description,
    bool? IsActive,
    string? LogoFileName = null,
    string? BannerFileName = null) : IRequest;

public class UpdateShopValidator : AbstractValidator<UpdateShopCommand>
{
    public UpdateShopValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Store name is required.")
            .MaximumLength(100).WithMessage("Store name must not exceed 100 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Store slug is required.")
            .MaximumLength(100).WithMessage("Store slug must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.LogoFileName)
            .MaximumLength(255).WithMessage("Logo file name must not exceed 255 characters.");

        RuleFor(x => x.BannerFileName)
            .MaximumLength(255).WithMessage("Banner file name must not exceed 255 characters.");
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
        shop.LogoFileName = request.LogoFileName;
        shop.BannerFileName = request.BannerFileName;

        if (request.IsActive.HasValue && currentUser.Role == UserRole.Admin)
        {
            shop.IsActive = request.IsActive.Value;
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
