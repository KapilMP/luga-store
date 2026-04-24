using SedaWears.Application.Features.Media.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SedaWears.API.Controllers;

public record RequestUploadUrlRequest(string ContentType, string FileName);

[ApiController]
[Route("[controller]")]
[Authorize]
public class MediaController(ISender mediator) : ControllerBase
{
    [HttpPost("upload-url")]
    public async Task<ActionResult<Uri>> RequestUploadUrl([FromBody] RequestUploadUrlRequest request)
    {
        var result = await mediator.Send(new RequestS3UploadUrlCommand(request.ContentType, request.FileName));
        return Ok(new { uploadUrl = result.ToString() });
    }
}
