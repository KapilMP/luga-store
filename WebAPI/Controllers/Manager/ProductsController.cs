using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.WebAPI.Controllers.Manager;

[ApiController]
[Route("manager/[controller]")]
[Authorize(Roles = Roles.Manager + "," + Roles.Admin)]
public class ProductsController(IApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStoreProducts()
    {
        // Manager focuses on the total store brand + collaborations
        var products = await context.Products.ToListAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> ManageProduct(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync(default);
        return CreatedAtAction(nameof(GetStoreProducts), new { id = product.Id }, product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOperationalProduct(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();

        context.Products.Remove(product);
        await context.SaveChangesAsync(default);
        return NoContent();
    }
}
