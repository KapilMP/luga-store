using SedaWears.Domain.Common;
using SedaWears.Domain.Enums;

namespace SedaWears.Domain.Entities;

public class ShopMember : BaseEntity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;


    public bool IsInvitationAccepted { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
