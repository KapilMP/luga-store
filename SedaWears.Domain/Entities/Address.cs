using SedaWears.Domain.Common;

namespace SedaWears.Domain.Entities;

public class Address : BaseAuditableEntity
{
    public string Label { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    // Relationship
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
