
namespace SedaWears.Domain.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}

public abstract class BaseEntity
{
    public int Id { get; set; }
}
