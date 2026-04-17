namespace LugaStore.Application.Common.Settings;

public record S3Config
{
    public string BucketName { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string Endpoint { get; init; } = string.Empty;
}
