namespace SedaWears.Application.Common.Settings;

public enum RateLimitingPolicies
{
    Global,
    Auth,
    ProductBrowsing,
    Search,
    Cart,
    Checkout,
    Payment,
    AccountActions,
    FileUpload
}
