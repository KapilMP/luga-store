using SedaWears.Domain.Common;

namespace SedaWears.Domain.Entities;

public class NewsletterSubscriber : BaseEntity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public string Email { get; set; } = string.Empty;
    public bool IsSubscribed { get; set; } = true;
    public string UnsubscribeToken { get; set; } = string.Empty;
}
