using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Purchases;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController
    (
        ApplicationDbContext _context,
        ILogger<PurchasesController> _logger
    ) : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> GetAllPurchase()
        {
            var purchases = _context.Purchases
                .ToList()
                .Select(x =>
                new PurchaseViewModel
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    PurchaseDate = x.PurchaseDate,
                    TotalCost = x.TotalCost,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate

                });

            return Task.FromResult<IActionResult>(Ok(purchases));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseById(int id)
        {
            var purchase = await _context.Purchases
                .Where(x => x.Id == id)
                .Select(x =>
                new PurchaseViewModel
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    PurchaseDate = x.PurchaseDate,
                    TotalCost = x.TotalCost,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate
                })
                .FirstOrDefaultAsync();
            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }

        [HttpPost]
        public async Task<IActionResult> PostPurchase([FromBody] PurchaseCreateRequest request)
        {
            var purchase = new Purchase
            {
                SupplierId = request.SupplierId,
                PurchaseDate = request.PurchaseDate,
                TotalCost = request.TotalCost, // Sử dụng hàm phương thức tính toán ...
                CreateDate = DateTime.UtcNow
            };

            _context.Purchases.Add(purchase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetPurchaseById), new { id = purchase.Id }, purchase);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] PurchaseUpdateRequest request)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            purchase.SupplierId = request.SupplierId;
            purchase.PurchaseDate = request.PurchaseDate;
            purchase.TotalCost = request.TotalCost; // Sử dụng hàm phương thức tính toán ...
            purchase.LastModifiedDate = DateTime.UtcNow;

            _context.Purchases.Update(purchase);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("trash")]
        public async Task<IActionResult> GetAllPurchaseInTrash()
        {
            var purchases = await _context.Purchases
                .Where(x => x.IsDeleted)
                .ToListAsync();

            return Ok(new
            {
                total = purchases.Count,
                items = purchases
            });
        }

        [HttpDelete("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            if (purchase.IsDeleted)
                return BadRequest();

            purchase.IsDeleted = true;

            var purchaseItemInPurchase = await _context.PurchaseItems
                .Where(x => x.PurchaseId == id)
                .ToListAsync();

            foreach (var purchaseItem in purchaseItemInPurchase)
            {
                purchaseItem.IsDeleted = true;
            }

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return Ok(result);

            return BadRequest();
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestorePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
                return NotFound();

            if (!purchase.IsDeleted)
                return BadRequest();

            purchase.IsDeleted = false;

            var purchaseItemInPurchase = await _context.PurchaseItems
               .Where(x => x.PurchaseId == id)
               .ToListAsync();

            foreach (var purchaseItem in purchaseItemInPurchase)
            {
                purchaseItem.IsDeleted = false;
            }

            var result = await _context.SaveChangesAsync();
            if (result > 0) 
                return Ok(result);

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> PermanentDeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if(purchase == null)
                return NotFound();

            if (!purchase.IsDeleted)
                return BadRequest("Purchase must be soft-deleted before permanent deletion");

            var purchaseItemInPurchase = await _context.PurchaseItems
               .Where(x => x.PurchaseId == id)
               .ToListAsync();

            foreach(var purchaseItem in purchaseItemInPurchase)
            {
                _context.PurchaseItems.Remove(purchaseItem);
            }    

            _context.Purchases.Remove(purchase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }
    }
}
