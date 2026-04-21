using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Services;

public class OriginContext(
    IHttpContextAccessor httpContextAccessor,
    RefreshTokenPathsConfig refreshConfig,
    IWebHostEnvironment environment) : IOriginContext
{
    public UserRole CurrentRole
    {
        get
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null) return UserRole.Customer;

            // 1. Development Override (Priority)
            if (environment.IsDevelopment())
            {
                var roleOverride = context.Request.Headers["X-Role-Override"].ToString().ToLower();
                if (!string.IsNullOrEmpty(roleOverride))
                {
                    return roleOverride switch
                    {
                        "admin" => UserRole.Admin,
                        "owner" => UserRole.Owner,
                        "manager" => UserRole.Manager,
                        "customer" => UserRole.Customer,
                        _ => UserRole.Customer
                    };
                }
            }

            // 2. Convention-based (Host)
            var host = context.Request.Host.Host.ToLower();

            if (host.StartsWith("admin.")) return UserRole.Admin;
            if (host.StartsWith("manager.")) return UserRole.Manager;
            if (host.StartsWith("owner.")) return UserRole.Owner;

            return UserRole.Customer;
        }
    }

    public string RefreshPath
    {
        get
        {
            return CurrentRole switch
            {
                UserRole.Admin => refreshConfig.AdminRefreshPath,
                UserRole.Owner => refreshConfig.OwnerRefreshPath,
                UserRole.Manager => refreshConfig.ManagerRefreshPath,
                _ => refreshConfig.CustomerRefreshPath
            };
        }
    }
}
