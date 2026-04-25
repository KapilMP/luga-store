using SedaWears.Application.Features.Users.Models;

namespace SedaWears.Application.Features.Auth.Models;

public record AuthResponse(string AccessToken, BaseUserRepresentation User);
public record ResetPasswordLinkResponse(string Link);
