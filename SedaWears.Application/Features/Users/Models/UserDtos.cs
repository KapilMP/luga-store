
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

public record AddressDto(
    int Id,
    string Label,
    string FullName,
    string Email,
    string Phone,
    string Street,
    string City,
    string ZipCode
);

public abstract record BaseUserDto(
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

public record AdminDto(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt
) : BaseUserDto(Id, PersonalInfo, Status, CreatedAt);

public record OwnerDto(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt,
    List<ShopSummary> Shops
) : BaseUserDto(Id, PersonalInfo, Status, CreatedAt);

public record ManagerDto(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt,
    List<ShopSummary> Shops
) : BaseUserDto(Id, PersonalInfo, Status, CreatedAt);

public record CustomerDto(
    int Id,
    PersonalInfo PersonalInfo,
    UserStatus Status,
    DateTime CreatedAt,
    List<AddressDto> SavedAddresses
) : BaseUserDto(Id, PersonalInfo, Status, CreatedAt);
