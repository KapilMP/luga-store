using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Settings;

namespace LugaStore.Infrastructure.ExternalServices;

public class OpeninaryService(OpeninaryConfig config, HttpClient httpClient) : IImageService
{
    private readonly OpeninaryConfig config = config;
    public async Task<string> UploadAvatarAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(stream);

        form.Add(fileContent, "files", fileName);
        form.Add(new StringContent("[\"w_400,h_400,c_fill,f_avif,q_70\"]"), "transformations");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{config.BaseUrl}/upload");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.ApiKey);
        request.Content = form;

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpeninaryResponse>(cancellationToken: cancellationToken);
        return result!.Files[0].Url;
    }

    private record OpeninaryResponse(List<OpeninaryFile> Files);
    private record OpeninaryFile(string Url);
}
