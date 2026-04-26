using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users;

public static class UserMapper
{
    public static BaseUserRepresentation ToUserRepresentation(this User user, DateTime? overrideCreatedAt = null)
    {
        var personalInfo = user.ToPersonalInfo();
        var status = new UserStatus(user.IsActive, user.EmailConfirmed);
        var createdAt = overrideCreatedAt ?? user.CreatedAt;

        return user.Role switch
        {
            UserRole.Admin => new AdminRepresentation(user.Id, personalInfo, status, createdAt),

            UserRole.Owner => new OwnerRepresentation(
                user.Id,
                personalInfo,
                status,
                createdAt,
                user.ShopMemberships
                    .Where(sm => sm.Shop.IsActive)
                    .Select(sm => new ShopSummary(
                        sm.ShopId,
                        sm.Shop.Name,
                        sm.Shop.LogoFileName
                    )).ToList()
            ),

            UserRole.Manager => new ManagerRepresentation(
                user.Id,
                personalInfo,
                status,
                createdAt,
                user.ShopMemberships
                    .Where(sm => sm.Shop.IsActive)
                    .Select(sm => new ShopSummary(
                        sm.ShopId,
                        sm.Shop.Name,
                        sm.Shop.LogoFileName
                    )).ToList()
            ),

            UserRole.Customer => new CustomerRepresentation(
                user.Id,
                personalInfo,
                status,
                createdAt,
                user.Addresses.Select(a => new AddressRepresentation(
                    a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode
                )).ToList()
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
