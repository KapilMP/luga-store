namespace LugaStore.Application.Features.Profile.Models;

public record UpdateProfileRequest(string FirstName, string LastName, string Phone, string? AvatarFileName);
public record AddressRequest(string Label, string FullName, string Email, string Phone, string Street, string City, string ZipCode);
