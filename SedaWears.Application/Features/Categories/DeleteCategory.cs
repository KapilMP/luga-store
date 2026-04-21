using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;

namespace SedaWears.Application.Features.Categories;

public record DeleteCategoryCommand(int Id, int? ShopId = null) : IRequest<Unit>;

public class DeleteCategoryHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await dbContext.Categories.FindAsync([request.Id], ct) ?? throw new NotFoundException("Category not found");
        if (request.ShopId.HasValue && category.ShopId != request.ShopId) throw new ForbiddenException("Unauthorized access to category");

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
