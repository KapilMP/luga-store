namespace LugaStore.Application.Profile.Models;

public record UpdateProfileRequest(string FirstName, string LastName, string Phone);
public record AddressRequest(string Label, string FullName, string Email, string Phone, string Street, string City, string ZipCode);
