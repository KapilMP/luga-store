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

public class UserProfileDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<AddressDto> Addresses { get; set; } = [];

    public static UserProfileDto From(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName ?? string.Empty,
        LastName = user.LastName ?? string.Empty,
        Email = user.Email ?? string.Empty,
        AvatarUrl = user.AvatarPath ?? string.Empty,
        Phone = user.PhoneNumber ?? string.Empty,
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
    };
}
