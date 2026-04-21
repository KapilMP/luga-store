using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;

namespace SedaWears.Infrastructure.ExternalServices;

public class S3Service(S3Config config) : IS3Service
{
    private readonly AmazonS3Client _s3Client = CreateClient(config);

    public Uri GetPreSignedUrl(string contentType, string fileName)
    {
        var key = $"public/{fileName}";

        var request = new GetPreSignedUrlRequest
        {
            BucketName = config.BucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15),
            ContentType = contentType
        };

        return new Uri(_s3Client.GetPreSignedURL(request));
    }

    public async Task DeleteFileAsync(string filename)
    {
        try
        {
            var publicRequest = new DeleteObjectRequest
            {
                BucketName = config.BucketName,
                Key = $"public/{filename}"
            };
            await _s3Client.DeleteObjectAsync(publicRequest);

            // Fire and forget cache deletion
            var cacheRequest = new DeleteObjectRequest
            {
                BucketName = config.BucketName,
                Key = $"cache/{filename}"
            };
            _ = _s3Client.DeleteObjectAsync(cacheRequest);
        }
        catch (Exception)
        {
            // Log error or handle as needed.
        }
    }

    private static AmazonS3Client CreateClient(S3Config config)
    {
        var s3Config = new AmazonS3Config
        {
            ForcePathStyle = true
        };

        if (!string.IsNullOrEmpty(config.Endpoint))
        {
            s3Config.ServiceURL = config.Endpoint;
        }
        else if (!string.IsNullOrEmpty(config.Region))
        {
            s3Config.RegionEndpoint = RegionEndpoint.GetBySystemName(config.Region);
        }

        return new AmazonS3Client(config.AccessKey, config.SecretKey, s3Config);
    }
}