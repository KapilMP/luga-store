using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Features.Categories;

public record DeleteCategoryCommand(int Id, int? PartnerId = null) : IRequest<Unit>;

public class DeleteCategoryHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await dbContext.Categories.FindAsync([request.Id], ct) ?? throw new NotFoundError("Category not found");
        if (request.PartnerId.HasValue && category.PartnerId != request.PartnerId) throw new ForbiddenError("Unauthorized access to category");

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
