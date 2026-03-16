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
        public async Task<IActionResult> GetAllPurchase()
        {
            _logger.LogInformation("Begin GetAllPurchase API");

            var purchases = await _context.Purchases
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
                .ToListAsync();

            _logger.LogInformation("Success GetAllPurchase API");

            return Ok(purchases);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseById(int id)
        {
            _logger.LogInformation("Begin GetPurchaseById API");

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
                _logger.LogError("Not found the purchase with id = {id}", id);

                return NotFound();
            }

            _logger.LogInformation("Return the purchase with id = {id}", id);

            return Ok(purchase);
        }

        [HttpPost]
        public async Task<IActionResult> PostPurchase([FromBody] PurchaseCreateRequest request)
        {
            _logger.LogInformation("Begin PostPurchase API");

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
                _logger.LogInformation("Success PostPurchase API with id = {id}", purchase.Id);

                return CreatedAtAction(nameof(GetPurchaseById), new { id = purchase.Id }, purchase);
            }
            else
            {
                _logger.LogError("Failed to create Purchase API");

                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] PurchaseUpdateRequest request)
        {
            _logger.LogInformation("Begin UpdatePurchase API");

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                _logger.LogError("Not found the purchase with id = {id}", id);

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
                _logger.LogInformation("Success to update the purchase with id = {id}", id);

                return NoContent();
            }
            else
            {
                _logger.LogError("Fail to update purchase with id = {id}", id);

                return BadRequest();
            }
        }

        [HttpGet("trash")]
        public async Task<IActionResult> GetAllPurchaseInTrash()
        {
            _logger.LogInformation("Begin GetAllPurchaseInTrash API");

            var purchases = await _context.Purchases
                .Where(x => x.IsDeleted)
                .ToListAsync();

            _logger.LogInformation("Success GetAllPurchaseInTrash API and return list purchase");

            return Ok(new
            {
                total = purchases.Count,
                items = purchases
            });
        }

        [HttpDelete("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeletePurchase(int id)
        {
            _logger.LogInformation("Begin SoftDeletePurchase API");

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                _logger.LogError("Not found the purchase with id = {id}", id);

                return NotFound();
            }

            if (purchase.IsDeleted)
            {
                _logger.LogError("Purchase already in the trash");

                return BadRequest("Purchase already in trash");
            }

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
            {
                _logger.LogInformation("SoftDeletePurchase success. Id = {id}", id);

                return Ok(result);
            }

            _logger.LogWarning("SoftDeletePurchase failed to save changes");

            return BadRequest();
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestorePurchase(int id)
        {
            _logger.LogInformation("Begin RestorePurchase API");

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                _logger.LogError("Not found the purchase with id = {id}", id);

                return NotFound();
            }

            if (!purchase.IsDeleted)
            {
                _logger.LogError("Not found the purchase in trash. Purchase id = {id}", id);

                return BadRequest();
            }

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
            {
                _logger.LogInformation("RestorePurchase success. Purchase id = {id}", id);

                return Ok(result);
            }

            _logger.LogWarning("RestorePurchase failed to save changes. Purchase id = {id}", id);

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> PermanentDeletePurchase(int id)
        {
            _logger.LogInformation("Begin PermanentDeletePurchase API");

            var purchase = await _context.Purchases.FindAsync(id);
            if(purchase == null)
            {
                _logger.LogError("Not found the purchase with id = {id}", id);

                return NotFound();
            }

            if (!purchase.IsDeleted)
            {
                _logger.LogError("Purchase must be soft-deleted before permanent deletion");

                return BadRequest("Purchase must be soft-deleted before permanent deletion");
            }

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
            {
                _logger.LogInformation("PermanentDeletePurchase success. Id = {id}", id);

                return NoContent();
            }

            _logger.LogWarning("PermanentDeletePurchase failed to save changes. Id = {id}", id);

            return BadRequest();
        }
    }
}
