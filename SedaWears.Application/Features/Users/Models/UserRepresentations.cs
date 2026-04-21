
namespace SedaWears.Application.Features.Users.Models;

public record PersonalInfo(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? AvatarFileName
);

public record UserStatus(
    bool IsActive,
    bool IsEmailConfirmed
);

public record AddressRepresentation(
    int Id,
    string Label,
    string FullName,
    string Email,
    string Phone,
    string Address, // Mapped from Street
    string City,
    string ZipCode
);

public record CreatorRepresentation(
    int Id,
    PersonalInfo PersonalInfo
);

public abstract record BaseUserRepresentation(
    int Id,
    DateTime CreatedAt,
    CreatorRepresentation? CreatedBy,
    PersonalInfo PersonalInfo,
    UserStatus Status
);

public record ShopSummary(
    int Id,
    string Name,
    string? LogoFileName
);

public record AdminRepresentation(
    int Id,
    DateTime CreatedAt,
    CreatorRepresentation? CreatedBy,
    PersonalInfo PersonalInfo,
    UserStatus Status
) : BaseUserRepresentation(Id, CreatedAt, CreatedBy, PersonalInfo, Status);

public record OwnerRepresentation(
    int Id,
    DateTime CreatedAt,
    CreatorRepresentation? CreatedBy,
    PersonalInfo PersonalInfo,
    UserStatus Status
) : BaseUserRepresentation(Id, CreatedAt, CreatedBy, PersonalInfo, Status);

public record ManagerRepresentation(
    int Id,
    DateTime CreatedAt,
    CreatorRepresentation? CreatedBy,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    List<ShopSummary> Shops
) : BaseUserRepresentation(Id, CreatedAt, CreatedBy, PersonalInfo, Status);

public record CustomerRepresentation(
    int Id,
    DateTime CreatedAt,
    CreatorRepresentation? CreatedBy,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    List<AddressRepresentation> SavedAddresses
) : BaseUserRepresentation(Id, CreatedAt, CreatedBy, PersonalInfo, Status);
