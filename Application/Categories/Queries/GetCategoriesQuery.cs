using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Categories;

namespace LugaStore.Application.Categories.Queries;

public record GetCategoriesQuery(int? PartnerId = null) : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler(ICategoryService categoryService) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (request.PartnerId.HasValue)
            return await categoryService.GetPartnerCategoriesAsync(request.PartnerId.Value, cancellationToken);

        return await categoryService.GetAllAsync(cancellationToken);
    }
}
