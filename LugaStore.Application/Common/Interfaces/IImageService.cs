namespace LugaStore.Application.Common.Interfaces;

public interface IImageService
{
    Task<string> UploadAvatarAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
}
