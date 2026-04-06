namespace LugaStore.Application.Features.Auth.Models;

public record LoginRequest(string Email, string Password);
public record GoogleLoginRequest(string IdToken);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Phone);
public record GuestCheckoutRequest(string Email, string FirstName, string LastName, string Phone);
public record ConfirmEmailRequest(string UserId, string Token);
