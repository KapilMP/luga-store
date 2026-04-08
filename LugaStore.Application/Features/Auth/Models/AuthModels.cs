using LugaStore.Application.Features.Users.Models;

namespace LugaStore.Application.Features.Auth.Models;

public record AuthResponse(string AccessToken, UserRepresentation User);
