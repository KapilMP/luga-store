using System.Security.Claims;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, string role);
    string GenerateRefreshToken(User user);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    string GenerateCsrfToken();
}
