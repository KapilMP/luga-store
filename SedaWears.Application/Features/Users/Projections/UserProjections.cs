using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Projections;

public static class UserProjections
{
    public static IQueryable<AdminDto> ProjectToAdmin(this IQueryable<User> query)
    {
        return query.Select(u => new AdminDto(
            u.Id,
            new PersonalInfo(u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.AvatarFileName),
            new UserStatus(u.IsActive, u.IsAdminInvitationAccepted ?? false),
            u.CreatedAt
        ));
    }

    public static IQueryable<OwnerDto> ProjectToOwner(this IQueryable<User> query)
    {
        return query.Select(u => new OwnerDto(
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

    public static IQueryable<ManagerDto> ProjectToManager(this IQueryable<User> query)
    {
        return query.Select(u => new ManagerDto(
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

    public static IQueryable<CustomerDto> ProjectToCustomer(this IQueryable<User> query)
    {
        return query.Select(u => new CustomerDto(
            u.Id,
            new PersonalInfo(u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.AvatarFileName),
            new UserStatus(u.IsActive, u.EmailConfirmed),
            u.CreatedAt,
            u.Addresses.Select(a => new AddressDto(
                a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode
            )).ToList()
        ));
    }
    public static BaseUserDto ToUserDto(this User user, DateTime? overrideCreatedAt = null)
    {
        var personalInfo = user.ToPersonalInfo();
        var status = new UserStatus(user.IsActive, user.EmailConfirmed);
        var createdAt = overrideCreatedAt ?? user.CreatedAt;

        return user.Role switch
        {
            UserRole.Admin => new AdminDto(user.Id, personalInfo, status, createdAt),

            UserRole.Owner => new OwnerDto(
                user.Id,
                personalInfo,
                status,
                createdAt,
                user.ShopMemberships?
                    .Where(sm => sm.Shop.IsActive)
                    .Select(sm => new ShopSummary(
                        sm.ShopId,
                        sm.Shop.Name,
                        sm.Shop.LogoFileName
                    )).ToList() ?? []
            ),

            UserRole.Manager => new ManagerDto(
                user.Id,
                personalInfo,
                status,
                createdAt,
                user.ShopMemberships?
                    .Where(sm => sm.Shop.IsActive)
                    .Select(sm => new ShopSummary(
                        sm.ShopId,
                        sm.Shop.Name,
                        sm.Shop.LogoFileName
                    )).ToList() ?? []
            ),

            UserRole.Customer => new CustomerDto(
                user.Id,
                personalInfo,
                status,
                createdAt,
                user.Addresses?.Select(a => new AddressDto(
                    a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode
                )).ToList() ?? []
            ),

            _ => throw new ArgumentException("Invalid role provided for mapping", user.Role.ToString())
        };
    }

    public static PersonalInfo ToPersonalInfo(this User user)
    {
        return new PersonalInfo(
            user.FirstName ?? string.Empty,
            user.LastName ?? string.Empty,
            user.Email,
            user.PhoneNumber,
            user.AvatarFileName
        );
    }
}
