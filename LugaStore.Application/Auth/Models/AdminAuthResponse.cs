namespace LugaStore.Application.Auth.Models;

public record AdminAuthResponse<T>(
    string AccessToken,
    T User);
