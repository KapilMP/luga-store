namespace LugaStore.Application.Common.Interfaces;

public interface IAuthService
{
    Task<bool> GuestCheckoutAsync(string email, string firstName, string lastName, string phone, CancellationToken ct);
}
