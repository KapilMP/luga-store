using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;
using System.Security.Claims;

namespace LugaStore.WebAPI.Controllers.Customer;

[ApiController]
[Route("customer/[controller]")]
[Authorize(Roles = Roles.Customer)]
public class AddressController(IUserService userService) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        var result = await userService.GetAddressesAsync(CurrentUserId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress(AddressDto dto)
    {
        var result = await userService.AddAddressAsync(CurrentUserId, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await userService.DeleteAddressAsync(CurrentUserId, id);
        return NoContent();
    }
}
