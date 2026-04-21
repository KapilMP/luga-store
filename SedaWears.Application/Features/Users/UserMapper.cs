using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users;

public static class UserMapper
{
    public static BaseUserRepresentation ToUserRepresentation(this User user)
    {
        var personalInfo = new PersonalInfo(
            user.FirstName ?? string.Empty,
            user.LastName ?? string.Empty,
            user.Email,
            user.PhoneNumber,
            user.AvatarFileName
        );

        var status = new UserStatus(user.IsActive);

        return user.Role switch
        {
            UserRole.Admin => new AdminRepresentation(user.Id, user.CreatedAt, personalInfo, status),

            UserRole.Owner => new OwnerRepresentation(
                user.Id,
                user.CreatedAt,
                personalInfo,
                status
            ),

            UserRole.Manager => new ManagerRepresentation(
                user.Id,
                user.CreatedAt,
                personalInfo,
                status,
                user.ManagedShops
                    .Where(ms => ms.Shop.IsActive)
                    .Select(ms => new ShopSummary(
                        ms.ShopId,
                        ms.Shop.Name,
                        ms.Shop.LogoFileName
                    )).ToList()
            ),

            UserRole.Customer => new CustomerRepresentation(
                user.Id,
                user.CreatedAt,
                personalInfo,
                status,
                user.Addresses.Select(a => new AddressRepresentation(
                    a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode
                )).ToList()
            ),

            _ => throw new ArgumentException("Invalid role provided for mapping", user.Role.ToString())
        };
    }
}
