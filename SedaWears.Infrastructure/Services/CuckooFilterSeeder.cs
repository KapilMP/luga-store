using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using StackExchange.Redis;

namespace SedaWears.Infrastructure.Services;

public static class CuckooFilterSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IUserCuckooFilter>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var cuckooFilter = scope.ServiceProvider.GetRequiredService<IUserCuckooFilter>();
        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        var db = redis.GetDatabase();

        logger.LogInformation("Initializing and Seeding Cuckoo Filters...");

        try
        {
            // 1. Reserve filters with production-ready capacities
            // Capacity: 10k for staff/admins, 100k for customers.
            // Expansion: 2 (default)
            await ReserveFilterSafe(db, "users:cuckoo:admin", 10000);
            await ReserveFilterSafe(db, "users:cuckoo:owner", 5000);
            await ReserveFilterSafe(db, "users:cuckoo:manager", 10000);
            await ReserveFilterSafe(db, "users:cuckoo:customer", 100000);

            // 2. Fetch all non-deleted users
            var users = await dbContext.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Select(u => new { u.Email, u.Role })
                .ToListAsync();

            int seededCount = 0;
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    await cuckooFilter.AddAsync(user.Email, user.Role);
                    seededCount++;
                }
            }

            logger.LogInformation("Cuckoo Filters seeded successfully with {Count} users.", seededCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "A critical error occurred while seeding Cuckoo Filters.");
        }
    }

    private static async Task ReserveFilterSafe(IDatabase db, string key, long capacity)
    {
        try
        {
            // CF.RESERVE key capacity [BUCKETSIZE] [MAXITERATIONS] [EXPANSION]
            // We use default values for others, but explicitly set capacity to prevent small defaults
            await db.CF().ReserveAsync(key, capacity);
        }
        catch (RedisException ex) when (ex.Message.Contains("already exists"))
        {
            // Filter already exists, which is fine
        }
    }
}
