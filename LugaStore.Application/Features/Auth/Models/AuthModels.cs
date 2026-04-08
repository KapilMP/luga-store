using LugaStore.Application.Features.Users.Models;

namespace LugaStore.Application.Features.Auth.Models;

public record AuthResponse(string AccessToken, UserRepresentation User);
public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
