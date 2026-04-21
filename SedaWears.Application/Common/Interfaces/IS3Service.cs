namespace SedaWears.Application.Common.Interfaces;

public interface IS3Service
{
    Uri GetPreSignedUrl(string contentType, string fileName);
    Task DeleteFileAsync(string fileName);
}
