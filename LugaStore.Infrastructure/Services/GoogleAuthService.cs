using Google.Apis.Auth;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Configurations;

using Microsoft.Extensions.Options;

namespace LugaStore.Infrastructure.Services;

public class GoogleAuthService(IOptions<GoogleConfig> googleOptions) : IGoogleAuthService
{
    private readonly GoogleConfig _googleConfig = googleOptions.Value;

    public async Task<GoogleUser?> ValidateTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings { Audience = [_googleConfig.ClientId] };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            if (payload == null) return null;
            return new GoogleUser(payload.Email, payload.GivenName ?? "", payload.FamilyName ?? "");
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
