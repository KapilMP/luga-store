using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LugaStore.Application.Common.Settings;

namespace LugaStore.Infrastructure.Configurations;

public class ConfigureJwtBearerOptions(JwtConfig jwtConfig) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtConfig _jwtConfig = jwtConfig;

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme) return;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfig.Issuer,
            ValidAudience = _jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    }

    public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);
}
