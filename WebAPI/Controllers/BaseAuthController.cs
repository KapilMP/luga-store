using LugaStore.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LugaStore.WebAPI.Controllers;

public abstract class BaseAuthController(IRefreshTokenPaths cookieSettings) : ControllerBase
{
    protected IRefreshTokenPaths RefreshTokenPaths => cookieSettings;

    protected void SetAuthCookies(string refreshToken, string refreshPath)
    {
        var csrf = Guid.NewGuid().ToString();

        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        Response.Cookies.Append("refreshCsrf", csrf, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    protected void ClearAuthCookies(string refreshPath)
    {
        var baseOptions = new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        Response.Cookies.Append("refreshToken", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = baseOptions.Secure,
            SameSite = baseOptions.SameSite,
            Path = baseOptions.Path,
            Expires = baseOptions.Expires
        });

        Response.Cookies.Append("refreshCsrf", "", new CookieOptions
        {
            HttpOnly = false,
            Secure = baseOptions.Secure,
            SameSite = baseOptions.SameSite,
            Path = baseOptions.Path,
            Expires = baseOptions.Expires
        });
    }
}
