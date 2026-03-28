using LugaStore.Domain.Entities;

namespace LugaStore.Application.Common.Models;

public class AddressDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

public class BaseUserProfile
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    protected static T MapBase<T>(User user, T dto) where T : BaseUserProfile
    {
        dto.Id = user.Id;
        dto.FirstName = user.FirstName ?? string.Empty;
        dto.LastName = user.LastName ?? string.Empty;
        dto.Email = user.Email ?? string.Empty;
        dto.AvatarUrl = user.AvatarPath ?? string.Empty;
        dto.Phone = user.PhoneNumber ?? string.Empty;
        return dto;
    }
}

public class AdminProfileDto : BaseUserProfile
{
    public static AdminProfileDto From(User user) => MapBase(user, new AdminProfileDto());
}

public class CustomerProfileDto : BaseUserProfile
{
    public bool IsEmailConfirmed { get; set; }
    public List<AddressDto> Addresses { get; set; } = [];

    public static CustomerProfileDto From(User user) => MapBase(user, new CustomerProfileDto
    {
        IsEmailConfirmed = user.EmailConfirmed,
        Addresses = [.. user.Addresses.Select(a => new AddressDto
        {
            Id = a.Id,
            Label = a.Label,
            FullName = a.FullName,
            Email = a.Email,
            Phone = a.Phone,
            Street = a.Street,
            City = a.City,
            ZipCode = a.ZipCode
        })]
    });
}

public class PartnerProfileDto : BaseUserProfile
{
    public static PartnerProfileDto From(User user) => MapBase(user, new PartnerProfileDto());
}

public class PartnerManagerProfileDto : BaseUserProfile
{
    public static PartnerManagerProfileDto From(User user) => MapBase(user, new PartnerManagerProfileDto());
}
