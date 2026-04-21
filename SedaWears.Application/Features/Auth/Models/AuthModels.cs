using SedaWears.Application.Features.Users.Models;

namespace SedaWears.Application.Features.Auth.Models;

public record AuthResponse(string AccessToken, BaseUserRepresentation User);
public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
public record AcceptInvitationRequest(string Email, string Token, string FirstName, string LastName, string Password, string Role);
public record InvitationDetailsResponse(string Email, string Role);
