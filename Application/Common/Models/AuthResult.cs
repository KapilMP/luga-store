namespace LugaStore.Application.Common.Models;

public class AuthResult
{
    public string AccessToken { get; set; } = string.Empty;
    public UserProfileDto User { get; set; } = default!;
}
