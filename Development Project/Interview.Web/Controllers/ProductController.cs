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
    [Route("api/v1/products")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        // NOTE: Sample Action
        [HttpGet]
        public Task<IActionResult> GetAllProducts()
        {
            return Task.FromResult((IActionResult)Ok(new object[] { }));
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Metadata = dto.Metadata ?? new Dictionary<string, string>()
            };

            // Attach categories (find or create)
            foreach (var catName in dto.Categories ?? Enumerable.Empty<string>())
            {
                var category = await _db.Categories.FirstOrDefaultAsync(c => c.Name == catName);
                if (category == null)
                {
                    category = new Category { Name = catName };
                    _db.Categories.Add(category);
                }

                product.ProductCategories.Add(new ProductCategory { Product = product, Category = category });
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return Created($"/api/v1/products/{product.Id}", product);
        }
    }
}
