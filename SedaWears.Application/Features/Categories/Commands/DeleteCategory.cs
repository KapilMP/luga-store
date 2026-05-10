using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id, int? ShopId = null) : IRequest;

public class DeleteCategoryHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        if (request.ShopId.HasValue)
        {
            var shopExists = await dbContext.Shops
                .AnyAsync(s => s.Id == request.ShopId.Value, ct);

            if (!shopExists)
                throw new NotFoundException("Shop not found.");

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
            throw new ForbiddenException("Only administrators can delete global categories.");
        }

        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.ShopId == request.ShopId, ct)
            ?? throw new NotFoundException("Category not found.");

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(ct);
    }
}
