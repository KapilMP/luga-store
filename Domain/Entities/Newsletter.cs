using System;
using LugaStore.Domain.Common;

namespace LugaStore.Domain.Entities;

public class Newsletter : BaseAuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public bool IsSubscribed { get; set; } = true;
    public string UnsubscribeToken { get; set; } = string.Empty;
}
