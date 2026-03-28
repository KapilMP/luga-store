namespace LugaStore.Infrastructure.Settings;

public interface IEmailSettings
{
    string Host { get; }
    int Port { get; }
    string Username { get; }
    string Password { get; }
    string FromName { get; }
    string FromEmail { get; }
    bool UseSsl { get; }
}
