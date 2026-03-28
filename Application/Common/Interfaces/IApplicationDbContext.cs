using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Newsletter> Newsletters { get; }
    DbSet<Address> Addresses { get; }
    DbSet<RestockSubscription> RestockSubscriptions { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<ProductSizeStock> ProductSizeStocks { get; }
    DbSet<ProductSale> ProductSales { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<PartnerManager> PartnerManagers { get; }
    DbSet<IdentityRole<int>> Roles { get; }
    DbSet<IdentityUserRole<int>> UserRoles { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
