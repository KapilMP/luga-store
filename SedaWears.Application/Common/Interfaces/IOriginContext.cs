using SedaWears.Domain.Enums;

namespace SedaWears.Application.Common.Interfaces;

public interface IOriginContext
{
    UserRole CurrentRole { get; }
    string RefreshPath { get; }
}
