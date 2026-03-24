using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Partner;

[ApiController]
[Route("partner/[controller]")]
[Authorize(Roles = Roles.Partner)]
public class ProductsController(
    IApplicationDbContext context,
    IUserService userService) : ControllerBase
{
    [HttpGet("my-creations")]
    public async Task<IActionResult> GetMyProducts()
    {
        var currentUserId = int.Parse(userService.UserId!);

        var products = await context.Products
            .Where(p => p.CreatorId == currentUserId)
            .ToListAsync();

        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsPartner(Product product)
    {
        product.CreatorId = int.Parse(userService.UserId!);

        context.Products.Add(product);
        await context.SaveChangesAsync(default);

        return CreatedAtAction(nameof(GetMyProducts), new { id = product.Id }, product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOwnedProduct(int id)
    {
        var currentUserId = int.Parse(userService.UserId!);
        var product = await context.Products.FindAsync(id);

        if (product == null) return NotFound();

        // Partner ownership check
        if (product.CreatorId != currentUserId && !User.IsInRole(Roles.Admin) && !User.IsInRole(Roles.Manager))
        {
            return Forbid("Only the partner owner or a manager can perform this action.");
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync(default);

        return NoContent();
    }
}
