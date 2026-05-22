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
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _db.Products.ToListAsync();
            return Ok(products);
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

            foreach (var categoryName in dto.Categories)
            {
                var existingCategory = await _db.Categories
                    .FirstOrDefaultAsync(c => c.Name == categoryName);

                if (existingCategory == null)
                {
                    existingCategory = new Category
                    {
                        Name = categoryName
                    };

                    _db.Categories.Add(existingCategory);
                }

                product.ProductCategories.Add(new ProductCategory
                {
                    Product = product,
                    Category = existingCategory
                });
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ProductController.GetAllProducts),
                new { id = product.Id },
                new 
                {
                    Id = product.Id,
                    Name = product.Name,
                }
                
            );
        }
    }
}
