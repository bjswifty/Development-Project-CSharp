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
    [Route("api/v1/inventory-transactions")]
    public class InventoryTransactionController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InventoryTransactionController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventoryTransaction(InventoryTransaction inventoryTransaction)
        {
            _db.InventoryTransactions.Add(inventoryTransaction);
            await _db.SaveChangesAsync();
            
            return Ok("Inventory transaction made successfully");
        }

        
    }
}
