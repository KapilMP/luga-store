using SedaWears.Domain.Enums;

namespace SedaWears.Application.Common.Interfaces;

public interface ICurrentUser
{
    int? Id { get; }
    int? ShopId { get; }
    UserRole? Role { get; }
}
