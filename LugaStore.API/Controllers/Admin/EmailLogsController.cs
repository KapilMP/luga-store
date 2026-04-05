using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Emails.Commands;
using LugaStore.Application.Emails.Queries;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.API.Controllers.Admin;

[ApiController]
[Route("admin/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class EmailLogsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] EmailStatus? status = null)
    {
        var result = await mediator.Send(new GetEmailLogsQuery(page, pageSize, status));
        return Ok(result);
    }

    [HttpPost("{id:int}/retry")]
    public async Task<IActionResult> Retry(int id)
    {
        var result = await mediator.Send(new RetryEmailCommand(id));
        if (!result) return NotFound();
        return Ok();
    }
}
