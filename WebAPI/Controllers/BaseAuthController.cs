using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LugaStore.WebAPI.Controllers;

public abstract class BaseAuthController : ControllerBase
{
    protected void SetAuthCookies(string refreshToken, string refreshCsrf, string refreshPath)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.Strict,
            Path = refreshPath, 
            Expires = DateTime.UtcNow.AddDays(7)
        };

        var csrfOptions = new CookieOptions
        {
            HttpOnly = false, // Accessible to JS
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = refreshPath, 
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, options);
        Response.Cookies.Append("refreshCsrf", refreshCsrf, csrfOptions);
    }
}
