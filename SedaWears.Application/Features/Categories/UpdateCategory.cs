using MediatR;
using FluentValidation;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;

namespace SedaWears.Application.Features.Categories;

public record UpdateCategoryCommand(int Id, string Name, string Slug, string? Description, int? ShopId = null) : IRequest<Unit>;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Category slug is required.")
            .MaximumLength(100).WithMessage("Category slug must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(5).WithMessage("Description must be at least 5 characters long.")
            .MaximumLength(300).WithMessage("Description must not exceed 300 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

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
