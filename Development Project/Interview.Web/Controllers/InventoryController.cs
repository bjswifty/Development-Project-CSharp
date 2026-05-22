using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Data;
using Interview.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory")]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InventoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory(Guid productId)
        {
            var products = await _db.Products.Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                    .ToListAsync();

            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                Metadata = p.Metadata,
                Categories = p.ProductCategories.Select(pc => pc.Category.Name)
            });
            return Ok(result);
        }
    }
}
