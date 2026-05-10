namespace SedaWears.Application.Common;

public static class AuthConstants
{
    public const string BaseScheme = "SedaWears.Base";
    public const string AdminScheme = "SedaWears.Admin";
    public const string OwnerScheme = "SedaWears.Owner";
    public const string ManagerScheme = "SedaWears.Manager";
    public const string CustomerScheme = "SedaWears.Customer";

    public static string GetSchemeForRole(string role) => role.ToLower() switch
    {
        "admin" => AdminScheme,
        "owner" => OwnerScheme,
        "manager" => ManagerScheme,
        _ => CustomerScheme
    };
}
