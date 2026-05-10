namespace SedaWears.Application.Features.Categories.Models;

public record CategoryRepresentation(int Id, string Name, string? Description, int DisplayOrder, bool IsActive);
