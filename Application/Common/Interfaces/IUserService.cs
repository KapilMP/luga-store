namespace LugaStore.Application.Common.Interfaces;

public interface IUserService
{
    string? UserId { get; }
    string? Role { get; }
    Task<bool> DeleteUserAsync(int userId);
}
