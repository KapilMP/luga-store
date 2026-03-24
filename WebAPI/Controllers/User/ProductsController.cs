using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.WebAPI.Controllers.User;

[ApiController]
[Route("user/[controller]")]
[AllowAnonymous] // Public access for E-commerce experience
public class ProductsController(IApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> BrowseAll()
    {
        // Marketplace view: See all active products
        var products = await context.Products
            .Select(p => new {
                p.Id,
                p.Name,
                p.Price,
                p.Description,
                IsCollaboration = p.CreatorId != null,
                CreatorName = p.Creator != null ? (p.Creator.FirstName + " " + p.Creator.LastName) : "Luga Brand"
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var product = await context.Products
            .Include(p => p.Creator)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        return Ok(product);
    }
}
