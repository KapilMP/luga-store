
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
    string Street,
    string City,
    string ZipCode
);

public record PartialUserRepresentation(
    int Id,
    PersonalInfo PersonalInfo
);

public abstract record BaseUserRepresentation(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt
);

public record ShopSummary(
    int Id,
    string Name,
    string? LogoFileName
);

public record AdminRepresentation(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt
) : BaseUserRepresentation(Id, PersonalInfo, Status, CreatedAt);

public record OwnerRepresentation(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt,
    List<ShopSummary> Shops
) : BaseUserRepresentation(Id, PersonalInfo, Status, CreatedAt);

public record ManagerRepresentation(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt,
    List<ShopSummary> Shops
) : BaseUserRepresentation(Id, PersonalInfo, Status, CreatedAt);

public record CustomerRepresentation(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt,
    List<AddressRepresentation> SavedAddresses
) : BaseUserRepresentation(Id, PersonalInfo, Status, CreatedAt);
