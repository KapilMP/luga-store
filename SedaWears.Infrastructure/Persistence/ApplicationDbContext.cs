using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Common;
using SedaWears.Domain.Entities;

namespace SedaWears.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : IdentityUserContext<User, int>(options), IApplicationDbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<RestockSubscription> RestockSubscriptions => Set<RestockSubscription>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductSizeStock> ProductSizeStocks => Set<ProductSizeStock>();
    public DbSet<ProductSale> ProductSales => Set<ProductSale>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<ShopMember> ShopMembers => Set<ShopMember>();
    DbSet<User> IApplicationDbContext.Users => Users;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Global Soft Delete Filter
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType).HasQueryFilter(GetIsDeletedFilter(entityType.ClrType));
            }
        }

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static LambdaExpression GetIsDeletedFilter(Type type)
    {
        var parameter = Expression.Parameter(type, "it");
        var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
        var falseConstant = Expression.Constant(false);
        var condition = Expression.Equal(property, falseConstant);
        return Expression.Lambda(condition, parameter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                if (entry.Entity is Shop shop) shop.IsActive = false;
                if (entry.Entity is User user) user.IsActive = false;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
