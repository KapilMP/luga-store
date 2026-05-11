namespace SedaWears.Application.Features.Users.Models;

public record UpdateUserRequest(string FirstName, string LastName, bool? IsActive);
public record ChangeUserPasswordRequest(string NewPassword);
public record InviteAdminRequest(string Email);
public record UpdateAdminActiveStatusRequest(bool IsActive);
