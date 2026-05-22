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

        [HttpGet("search")]
        public async Task<IActionResult> Search(
    string? query,
    string? category,
    string? metaKey,
    string? metaValue)
        {
            var productsQuery = _db.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(query) ||
                    p.Description.Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                productsQuery = productsQuery.Where(p =>
                    p.ProductCategories.Any(pc =>
                        pc.Category.Name == category));
            }

            var products = await productsQuery.ToListAsync();

            // metadata filtering MUST happen in memory
            if (!string.IsNullOrWhiteSpace(metaKey) &&
                !string.IsNullOrWhiteSpace(metaValue))
            {
                products = products.Where(p =>
                    p.Metadata != null &&
                    p.Metadata.TryGetValue(metaKey, out var value) &&
                    value == metaValue)
                    .ToList();
            }

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
