namespace LugaStore.Application.Common.Models;

public record AuthenticationResponse(
    bool IsSuccess, 
    string? AccessToken, 
    string? RefreshToken = null, // Optional for non-browser clients
    string? ErrorMessage = null);
