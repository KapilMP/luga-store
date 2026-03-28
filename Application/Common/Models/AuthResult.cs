namespace LugaStore.Application.Common.Models;

public class AuthResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public BaseUserProfile User { get; set; } = default!;
}
