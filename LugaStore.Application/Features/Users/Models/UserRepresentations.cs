using LugaStore.Application.Common.Settings;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Features.Users.Models;

public record UserRepresentation(
    int Id,
    DateTime CreatedAt,
    string? Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? AvatarPath,
    bool IsActive,
    bool EmailConfirmed
)
{
    public static UserRepresentation ToUserRepresentation(User user) => new(
        user.Id, 
        user.Created, 
        user.Email, 
        user.FirstName, 
        user.LastName, 
        user.PhoneNumber, 
        user.AvatarPath, 
        user.IsActive, 
        user.EmailConfirmed
    );
}

public record AddressRepresentation(
    int Id, string Label, string FullName, string Email, string Phone, string Street, string City, string ZipCode
)
{
    public static AddressRepresentation FromEntity(Address a) => new(
        a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode
    );
}

public record CustomerRepresentation(
    int Id,
    DateTime CreatedAt,
    string? Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? AvatarPath,
    bool IsActive,
    bool EmailConfirmed,
    List<AddressRepresentation> Addresses
) : UserRepresentation(Id, CreatedAt, Email, FirstName, LastName, Phone, AvatarPath, IsActive, EmailConfirmed)
{
    public static CustomerRepresentation ToCustomerRepresentation(User user) => new(
        user.Id, user.Created, user.Email, user.FirstName, user.LastName, user.PhoneNumber, user.AvatarPath, user.IsActive, user.EmailConfirmed,
        user.Addresses.Select(AddressRepresentation.FromEntity).ToList()
    );
}

public record PartnerRepresentation(
    int Id,
    DateTime CreatedAt,
    string? Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? AvatarPath,
    bool IsActive,
    bool EmailConfirmed
) : UserRepresentation(Id, CreatedAt, Email, FirstName, LastName, Phone, AvatarPath, IsActive, EmailConfirmed)
{
    public static PartnerRepresentation ToPartnerRepresentation(User user) => new(
        user.Id, user.Created, user.Email, user.FirstName, user.LastName, user.PhoneNumber, user.AvatarPath, user.IsActive, user.EmailConfirmed
    );
}

public record PartnerManagerRepresentation(
    int Id,
    DateTime CreatedAt,
    string? Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? AvatarPath,
    bool IsActive,
    bool EmailConfirmed
) : UserRepresentation(Id, CreatedAt, Email, FirstName, LastName, Phone, AvatarPath, IsActive, EmailConfirmed)
{
    public static PartnerManagerRepresentation ToPartnerManagerRepresentation(User user) => new(
        user.Id, user.Created, user.Email, user.FirstName, user.LastName, user.PhoneNumber, user.AvatarPath, user.IsActive, user.EmailConfirmed
    );
}

public record AdminRepresentation(
    int Id,
    DateTime CreatedAt,
    string? Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? AvatarPath,
    bool IsActive,
    bool EmailConfirmed
) : UserRepresentation(Id, CreatedAt, Email, FirstName, LastName, Phone, AvatarPath, IsActive, EmailConfirmed)
{
    public static AdminRepresentation ToAdminRepresentation(User user) => new(
        user.Id, user.Created, user.Email, user.FirstName, user.LastName, user.PhoneNumber, user.AvatarPath, user.IsActive, user.EmailConfirmed
    );
}
