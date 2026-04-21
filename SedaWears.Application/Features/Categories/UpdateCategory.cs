using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;

namespace SedaWears.Application.Features.Categories;

public record UpdateCategoryCommand(int Id, string Name, string Slug, string? Description, int? ShopId = null) : IRequest<Unit>;

public class UpdateCategoryHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await dbContext.Categories.FindAsync([request.Id], ct) ?? throw new NotFoundException("Category not found");
        if (request.ShopId.HasValue && category.ShopId != request.ShopId) throw new ForbiddenException("Unauthorized access to category");

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.Description = request.Description;
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
