using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Users.Projections;

public static class UserProjections
{
    public static IQueryable<AdminRepresentation> ProjectToAdmin(this IQueryable<User> query)
    {
        return query.Select(u => new AdminRepresentation(
            u.Id,
            new PersonalInfo(u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.AvatarFileName),
            new UserStatus(u.IsActive, u.IsAdminInvitationAccepted ?? false),
            u.CreatedAt
        ));
    }

    public static IQueryable<OwnerRepresentation> ProjectToOwner(this IQueryable<User> query)
    {
        return query.Select(u => new OwnerRepresentation(
            u.Id,
            new PersonalInfo(u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.AvatarFileName),
            new UserStatus(u.IsActive, u.EmailConfirmed),
            u.CreatedAt,
            u.ShopMemberships
                .Where(sm => sm.Shop.IsActive)
                .Select(sm => new ShopSummary(
                    sm.ShopId,
                    sm.Shop.Name,
                    sm.Shop.LogoFileName
                )).ToList()
        ));
    }

    public static IQueryable<ManagerRepresentation> ProjectToManager(this IQueryable<User> query)
    {
        return query.Select(u => new ManagerRepresentation(
            u.Id,
            new PersonalInfo(u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.AvatarFileName),
            new UserStatus(u.IsActive, u.EmailConfirmed),
            u.CreatedAt,
            u.ShopMemberships
                .Where(sm => sm.Shop.IsActive)
                .Select(sm => new ShopSummary(
                    sm.ShopId,
                    sm.Shop.Name,
                    sm.Shop.LogoFileName
                )).ToList()
        ));
    }

    public static IQueryable<CustomerRepresentation> ProjectToCustomer(this IQueryable<User> query)
    {
        return query.Select(u => new CustomerRepresentation(
            u.Id,
            new PersonalInfo(u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.AvatarFileName),
            new UserStatus(u.IsActive, u.EmailConfirmed),
            u.CreatedAt,
            u.Addresses.Select(a => new AddressRepresentation(
                a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode
            )).ToList()
        ));
    }
}
