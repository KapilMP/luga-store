
namespace LugaStore.Application.Features.UserManagement.Models;

public record PersonalInfoRepresentation(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? Phone = null,
    string? AvatarPath = null
);


public record CustomerPersonalInfoRepresentation(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? Phone = null,
    string? AvatarPath = null,
    List<AddressRepresentation>? Addresses = null
);

public record StatusRepresentation(
    bool IsActive
);

public record VerificationRepresentation(
    bool EmailConfirmed
);

public record AdminRepresentation(
    int Id,
    DateTime CreatedAt,
    PersonalInfoRepresentation? PersonalInfo = null,
    StatusRepresentation? Status = null
);

public record CustomerRepresentation(
    int Id,
    VerificationRepresentation? Verification,
    CustomerPersonalInfoRepresentation? PersonalInfo,
    StatusRepresentation? Status
);

public record PartnerRepresentation(
    int Id,
    DateTime CreatedAt,
    PersonalInfoRepresentation? PersonalInfo = null,
    StatusRepresentation? Status = null
);

public record PartnerManagerRepresentation(
    int Id,
    DateTime CreatedAt,
    PersonalInfoRepresentation? PersonalInfo = null,
    StatusRepresentation? Status = null
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
