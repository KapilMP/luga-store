namespace LugaStore.Application.Categories;

public record CategoryDto(int Id, string Name, string? Description, int DisplayOrder);

public record CategoryUpsertRequest(string Name, string? Description);
public record CategoryReorderRequest(List<OrderItemDto> Orders);
public record OrderItemDto(int Id, int DisplayOrder);
