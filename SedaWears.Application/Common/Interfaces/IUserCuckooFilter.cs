using SedaWears.Domain.Enums;

namespace SedaWears.Application.Common.Interfaces;

public interface IUserCuckooFilter
{
    Task<bool> ExistsAsync(string email, UserRole role);
    Task AddAsync(string email, UserRole role);
    Task RemoveAsync(string email, UserRole role);
    Task<bool> AddIfNotExistsAsync(string email, UserRole role);
}
