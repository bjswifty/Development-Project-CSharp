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
        public async Task<IActionResult> GetInventoryByProductId(Guid productId)
        {
            var inventory = await _db.InventoryTransactions.Where(t => t.ProductId == productId).ToListAsync();
            var onHandQuantity = inventory.Sum(i => i.Quantity);
            return Ok(onHandQuantity);
        }


    }
}
