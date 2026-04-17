namespace LugaStore.Application.Common.Interfaces;

public interface IS3Service
{
    Uri GetPreSignedUrl(string contentType, string? fileName = null);
    Task DeletePhotoAsync(string path);
}
