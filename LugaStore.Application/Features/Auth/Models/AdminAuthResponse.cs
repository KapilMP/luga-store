namespace LugaStore.Application.Features.Auth.Models;

public record AdminAuthResponse<T>(
    string AccessToken,
    T User);
