using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Application.Profile.Models;
using LugaStore.Application.Profile.Commands;
using LugaStore.Application.Profile.Queries;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
[Authorize(Roles = Roles.Customer)]
public class AddressController(ISender mediator) : LugaStoreControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AddressRepresentation>>> GetAddresses()
    {
        return await mediator.Send(new GetAddressesQuery(GetUserId()));
    }

    [HttpPost]
    public async Task<ActionResult<AddressRepresentation>> AddAddress(AddressRequest request)
    {
        var representation = new AddressRepresentation(
            0, request.Label, request.FullName, request.Email, request.Phone, request.Street, request.City, request.ZipCode);
        
        return await mediator.Send(new AddAddressCommand(GetUserId(), representation));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAddress(int id)
    {
        await mediator.Send(new DeleteAddressCommand(GetUserId(), id));
        return NoContent();
    }
}
