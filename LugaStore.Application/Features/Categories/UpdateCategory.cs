using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Features.Categories;

public record UpdateCategoryCommand(int Id, string Name, string Slug, string? Description, int? PartnerId = null) : IRequest<Unit>;

public class UpdateCategoryHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await dbContext.Categories.FindAsync([request.Id], ct) ?? throw new NotFoundError("Category not found");
        if (request.PartnerId.HasValue && category.PartnerId != request.PartnerId) throw new ForbiddenError("Unauthorized access to category");

        category.Name = request.Name;
        category.Slug = request.Slug;
        category.Description = request.Description;
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
