using LugaStore.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LugaStore.WebAPI.Controllers;

public abstract class BaseAuthController(ICookieSettings cookieSettings) : ControllerBase
{
    protected ICookieSettings CookieSettings => cookieSettings;

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
        var expired = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = refreshPath,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        Response.Cookies.Append("refreshToken", "", expired);
        Response.Cookies.Append("refreshCsrf", "", expired with { HttpOnly = false });
    }
}
