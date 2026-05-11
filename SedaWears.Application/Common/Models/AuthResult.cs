using SedaWears.Application.Features.Users.Models;

namespace SedaWears.Application.Common.Models;

public class AuthResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public BaseUserDto User { get; set; } = default!;
}
