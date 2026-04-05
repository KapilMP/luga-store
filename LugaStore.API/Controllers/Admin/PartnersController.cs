using System.Collections.Generic;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Invitations.Commands;
using LugaStore.Application.UserManagement.Commands;
using LugaStore.Application.UserManagement.Queries;
using LugaStore.Application.Categories.Commands;
using LugaStore.Application.Categories.Queries;
using LugaStore.Application.Common.Models;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class PartnersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<PartnerRepresentation>>> GetPartners(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? invited = null,
        [FromQuery] bool? isActive = null)
    {
        return await mediator.Send(new GetPartnersQuery(pageNumber, pageSize, invited, isActive));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartnerRepresentation>> GetPartner(int id)
    {
        return await mediator.Send(new GetPartnerQuery(id));
    }

    public record InvitePartnerRequest(string Email);

    [HttpPost("invite")]
    public async Task<ActionResult<string>> InvitePartner(InvitePartnerRequest request)
    {
        await mediator.Send(new InvitePartnerCommand(request.Email));
        return Ok("Invitation sent.");
    }

    [HttpPost("{id:int}/resend-invitation")]
    public async Task<ActionResult<string>> ResendInvitation(int id)
    {
        await mediator.Send(new ResendPartnerInvitationCommand(id));
        return Ok("Invitation resent.");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePartner(int id)
    {
        await mediator.Send(new DeletePartnerCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<ActionResult> ActivatePartner(int id)
    {
        await mediator.Send(new ActivatePartnerCommand(id));
        return Ok();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult> DeactivatePartner(int id)
    {
        await mediator.Send(new DeactivatePartnerCommand(id));
        return Ok();
    }

    [HttpGet("{partnerId}/categories")]
    public async Task<ActionResult<List<LugaStore.Application.Categories.CategoryDto>>> GetAllCategories(int partnerId, CancellationToken ct)
    {
        return await mediator.Send(new GetCategoriesQuery(partnerId), ct);
    }

    [HttpPost("{partnerId}/categories")]
    public async Task<ActionResult<int>> Create(int partnerId, LugaStore.Application.Categories.CategoryUpsertRequest request, CancellationToken ct)
    {
        return await mediator.Send(new CreateCategoryCommand(request.Name, request.Slug, request.Description, partnerId), ct);
    }

    [HttpPut("{partnerId}/categories/{id:int}")]
    public async Task<ActionResult> Update(int partnerId, int id, LugaStore.Application.Categories.CategoryUpsertRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Slug, request.Description, partnerId), ct);
        return Ok();
    }

    [HttpDelete("{partnerId}/categories/{id:int}")]
    public async Task<ActionResult> Delete(int partnerId, int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryCommand(id, partnerId), ct);
        return Ok();
    }

    [HttpPost("{partnerId}/categories/reorder")]
    public async Task<ActionResult> Reorder(int partnerId, LugaStore.Application.Categories.CategoryReorderRequest request, CancellationToken ct)
    {
        var orders = request.Orders.Select(o => new LugaStore.Application.Categories.Commands.CategoryOrderDto(o.Id, o.DisplayOrder)).ToList();
        await mediator.Send(new ReorderCategoriesCommand(orders, partnerId), ct);
        return Ok();
    }
}
