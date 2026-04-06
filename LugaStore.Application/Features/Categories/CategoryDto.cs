namespace LugaStore.Application.Features.Categories;

public record CategoryDto(int Id, string Name, string Slug, string? Description, int DisplayOrder);

public record CategoryUpsertRequest(string Name, string Slug, string? Description);
public record CategoryReorderRequest(List<OrderItemDto> Orders);
public record OrderItemDto(int Id, int DisplayOrder);
