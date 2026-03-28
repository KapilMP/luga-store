using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Application.Categories;

namespace LugaStore.Application.Categories.Commands;

public record CreateCategoryCommand(string Name, string? Description, int? PartnerId = null) : IRequest<int>;
public record UpdateCategoryCommand(int Id, string Name, string? Description, int? PartnerId = null) : IRequest<Unit>;
public record DeleteCategoryCommand(int Id, int? PartnerId = null) : IRequest<Unit>;
public record ReorderCategoriesCommand(List<CategoryOrderDto> Orders, int? PartnerId = null) : IRequest<Unit>;
public record CategoryOrderDto(int Id, int DisplayOrder);

public class CategoryCommandHandlers(ICategoryService categoryService) : 
    IRequestHandler<CreateCategoryCommand, int>,
    IRequestHandler<UpdateCategoryCommand, Unit>,
    IRequestHandler<DeleteCategoryCommand, Unit>,
    IRequestHandler<ReorderCategoriesCommand, Unit>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.PartnerId.HasValue)
            return await categoryService.CreatePartnerCategoryAsync(request.PartnerId.Value, request.Name, request.Description, cancellationToken);
        
        return await categoryService.CreateAsync(request.Name, request.Description, cancellationToken);
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.PartnerId.HasValue)
            await categoryService.UpdatePartnerCategoryAsync(request.PartnerId.Value, request.Id, request.Name, request.Description, cancellationToken);
        else 
            await categoryService.UpdateAsync(request.Id, request.Name, request.Description, cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.PartnerId.HasValue)
            await categoryService.DeletePartnerCategoryAsync(request.PartnerId.Value, request.Id, cancellationToken);
        else 
            await categoryService.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> Handle(ReorderCategoriesCommand request, CancellationToken cancellationToken)
    {
        var orders = request.Orders.Select(o => (o.Id, o.DisplayOrder)).ToList();
        
        if (request.PartnerId.HasValue)
            await categoryService.ReorderPartnerCategoriesAsync(request.PartnerId.Value, orders, cancellationToken);
        else 
            await categoryService.ReorderAsync(orders, cancellationToken);

        return Unit.Value;
    }
}
