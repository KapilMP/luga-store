namespace LugaStore.WebAPI.Dtos;

public record AuthTokenDto(string AccessToken);

public record CurrentUserDto(int UserId, string Role);
