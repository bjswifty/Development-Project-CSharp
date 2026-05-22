using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Interview.Web.Data;
using Interview.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Interview.Web.Controllers
{
    [Route("api/v1/inventory-transactions")]
    public class InventoryTransactionController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InventoryTransactionController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventoryTransaction([FromBody] InventoryTransactionCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            if (dto.Quantity == 0)
                return BadRequest("Quantity must not be zero.");

            var productExists = await _db.Products.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists)
                return NotFound($"Product '{dto.ProductId}' was not found.");

            var transaction = new InventoryTransaction
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Type = dto.Type
            };

            _db.InventoryTransactions.Add(transaction);
            await _db.SaveChangesAsync();

            return Created(
                $"/api/v1/inventory-transactions/{transaction.TransactionId}",
                new
                {
                    transaction.TransactionId,
                    transaction.ProductId,
                    transaction.Quantity,
                    transaction.Type,
                    transaction.CreatedAt
                });
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetInventoryTransactionsByProductId(Guid productId)
        {
            var transactions = await _db.InventoryTransactions.Where(t => t.ProductId == productId).ToListAsync();
            return Ok(transactions);
        }
    }
}
