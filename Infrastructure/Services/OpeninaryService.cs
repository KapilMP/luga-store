using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Infrastructure.Settings;

namespace LugaStore.Infrastructure.Services;

public class OpeninaryService(IOpeninarySettings settings, HttpClient httpClient) : IImageService
{
    public async Task<string> UploadAvatarAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(stream);

        form.Add(fileContent, "files", fileName);
        form.Add(new StringContent("[\"w_400,h_400,c_fill,f_avif,q_70\"]"), "transformations");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{settings.BaseUrl}/upload");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.ApiKey);
        request.Content = form;

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpeninaryResponse>(cancellationToken: cancellationToken);
        return result!.Files[0].Url;
    }

    private record OpeninaryResponse(List<OpeninaryFile> Files);
    private record OpeninaryFile(string Url);
}
