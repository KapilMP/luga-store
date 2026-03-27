namespace LugaStore.WebAPI.Dtos;

public record LoginRequest(string Email, string Password);
public record InvitePartnerManagerRequest(string Email, string FirstName, string LastName);
