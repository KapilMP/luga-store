using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Application.Features.Profile.Models;
using LugaStore.Application.Features.Profile.Commands;
using LugaStore.Application.Features.Profile.Queries;
using LugaStore.Domain.Common;
using Microsoft.AspNetCore.RateLimiting;
using LugaStore.Application.Common.Settings;

namespace LugaStore.API.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
[Authorize(Roles = Roles.Customer)]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public class AddressController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AddressRepresentation>>> GetAddresses()
    {
        return await mediator.Send(new GetAddressesQuery());
    }

    [HttpPost]
    public async Task<ActionResult<AddressRepresentation>> AddAddress(AddressRequest request)
    {
        var representation = new AddressRepresentation(
            0, request.Label, request.FullName, request.Email, request.Phone, request.Street, request.City, request.ZipCode);
        
        return await mediator.Send(new AddAddressCommand(representation));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAddress(int id)
    {
        await mediator.Send(new DeleteAddressCommand(id));
        return NoContent();
    }
}
