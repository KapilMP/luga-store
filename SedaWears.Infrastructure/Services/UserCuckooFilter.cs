using Microsoft.Extensions.Logging;
using NRedisStack.RedisStackCommands;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using StackExchange.Redis;

namespace SedaWears.Infrastructure.Services;

public class UserCuckooFilter(IConnectionMultiplexer redis, ILogger<UserCuckooFilter> logger) : IUserCuckooFilter
{
    private readonly IDatabase _db = redis.GetDatabase();

    private string GetKey(UserRole role) => $"users:cuckoo:{role.ToString().ToLower()}";

    public async Task<bool> ExistsAsync(string email, UserRole role)
    {
        try
        {
            return await _db.CF().ExistsAsync(GetKey(role), email);
        }
        catch (Exception ex)
        {
            // Fail-open: If Redis is down, return true so we fallback to a safe DB check
            logger.LogWarning(ex, "Redis Cuckoo Filter check failed for {Email}. Falling back to DB.", email);
            return true;
        }
    }

    public async Task AddAsync(string email, UserRole role)
    {
        try
        {
            await _db.CF().AddAsync(GetKey(role), email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add {Email} to Redis Cuckoo Filter.", email);
        }
    }

    public async Task RemoveAsync(string email, UserRole role)
    {
        try
        {
            await _db.CF().DelAsync(GetKey(role), email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to remove {Email} from Redis Cuckoo Filter.", email);
        }
    }

    public async Task<bool> AddIfNotExistsAsync(string email, UserRole role)
    {
        try
        {
            return await _db.CF().AddNXAsync(GetKey(role), email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to AddNX {Email} to Redis Cuckoo Filter.", email);
            return false;
        }
    }
}
