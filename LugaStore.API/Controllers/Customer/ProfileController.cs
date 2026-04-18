using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Profile.Commands;
using LugaStore.Application.Features.Profile.Queries;
using LugaStore.Application.Features.Profile.Models;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Common;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Common.Settings;

namespace LugaStore.API.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
[Authorize(Roles = Roles.Customer)]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class ProfileController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CustomerRepresentation>> GetProfile()
    {
        return await mediator.Send(new GetCustomerProfileQuery());
    }

    [HttpPatch]
    public async Task<ActionResult<CustomerRepresentation>> UpdateProfile(UpdateProfileRequest request)
    {
        return await mediator.Send(new UpdateCustomerProfileCommand(request.FirstName, request.LastName, request.Phone, request.AvatarFileName));
    }

    [HttpGet("avatar/upload-url")]
    public async Task<ActionResult<ImageUploadUrlResponse>> GetAvatarUploadUrl([FromQuery] string fileName, [FromQuery] string contentType)
    {
        return await mediator.Send(new GetCustomerAvatarUploadUrlCommand(fileName, contentType));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAccount()
    {
        await mediator.Send(new DeleteAccountCommand());
        return NoContent();
    }
}
