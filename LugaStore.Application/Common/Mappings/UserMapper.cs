using System.Linq;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Common.Mappings;

public static class UserMapper
{
    public static AdminRepresentation ToAdminRepresentation(this User user)
    {
        return new AdminRepresentation(
            user.Id,
            user.Created,
            new PersonalInfoRepresentation(user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.AvatarPath),
            new StatusRepresentation(user.IsActive)
        );
    }

    public static CustomerRepresentation ToCustomerRepresentation(this User user)
    {
        var addresses = user.Addresses?.Select(a => new AddressRepresentation(
                a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode)).ToList();

        return new CustomerRepresentation(
            Id: user.Id,
            Verification: new VerificationRepresentation(user.EmailConfirmed),
            PersonalInfo: new CustomerPersonalInfoRepresentation(user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.AvatarPath, addresses),
            Status: new StatusRepresentation(user.IsActive)
        );
    }

    public static PartnerRepresentation ToPartnerRepresentation(this User user)
    {
        return new PartnerRepresentation(
            user.Id,
            user.Created,
            new PersonalInfoRepresentation(user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.AvatarPath),
            new StatusRepresentation(user.IsActive)
        );
    }

    public static PartnerManagerRepresentation ToPartnerManagerRepresentation(this User user)
    {
        return new PartnerManagerRepresentation(
            user.Id,
            user.Created,
            new PersonalInfoRepresentation(user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.AvatarPath),
            new StatusRepresentation(user.IsActive)
        );
    }
}
