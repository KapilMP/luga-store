using LugaStore.Application.Features.Users.Models;
using LugaStore.Application.Features.Users.Models;

namespace LugaStore.Application.Features.Auth.Models;

public record AuthResponse(string AccessToken, string RefreshToken, UserRepresentation User);
