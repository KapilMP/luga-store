using System.Security.Claims;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    string GenerateCsrfToken();
}
