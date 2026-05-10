using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, string? Description, int? ShopId = null) : IRequest;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(5).WithMessage("Description must be at least 5 characters.")
            .MaximumLength(100).WithMessage("Description must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class CreateCategoryHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<CreateCategoryCommand>
{
    public async Task Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (request.ShopId.HasValue)
        {
            var shop = await dbContext.Shops
                .FirstOrDefaultAsync(s => s.Id == request.ShopId.Value, ct)
                ?? throw new NotFoundException("Shop not found.");

            if (currentUser.Role != UserRole.Admin)
            {
                var isMember = await dbContext.ShopMembers
                    .AnyAsync(sm => sm.UserId == currentUser.Id && sm.ShopId == request.ShopId.Value, ct);

                if (!isMember)
                    throw new NotFoundException("Shop not found.");
            }
        }
        else if (currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only administrators can create global categories.");
        }

        var exists = await dbContext.Categories
            .AnyAsync(c => EF.Functions.ILike(c.Name, request.Name) && c.ShopId == request.ShopId, ct);

        if (exists)
            throw new BadRequestException("Category with this name already exists.");

        var finalOrder = (await dbContext.Categories
            .Where(c => c.ShopId == request.ShopId)
            .MaxAsync(c => (int?)c.DisplayOrder, ct) ?? 0) + 1;

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            DisplayOrder = finalOrder,
            ShopId = request.ShopId
        };
        dbContext.Categories.Add(category);

        await dbContext.SaveChangesAsync(ct);
    }
}
