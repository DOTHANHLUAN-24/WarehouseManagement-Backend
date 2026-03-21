using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Purchases;
using WarehouseManagement.ViewModels.Systems;

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
        #region Purchases

        /// <summary>
        /// Get list purchase in the system
        /// </summary>
        /// <returns>Return list purchase</returns>
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

        [HttpGet("filter")]
        public async Task<IActionResult> GetPurchasesPaging
        (
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Purchases.AsQueryable();

            if (fromDate != null)
            {
                query = query.Where(x => x.CreateDate >= fromDate);
            }

            if (toDate != null)
            {
                query = query.Where(x => x.CreateDate <= toDate);
            }

            var totalRecords = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var data = items.Select(x => new PurchaseViewModel
            {
                Id = x.Id,
                SupplierId = x.SupplierId,
                PurchaseDate = x.PurchaseDate,
                TotalCost = x.TotalCost,
                CreateDate = x.CreateDate,
                LastModifiedDate = x.LastModifiedDate
            }).ToList();

            var pagination = new Pagination<PurchaseViewModel>
            {
                Items = data,
                TotalRecords = totalRecords,
            };

            return Ok(pagination);
        }

        /// <summary>
        /// Get purchase by id
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <returns>The purchase or not found</returns>
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

        /// <summary>
        /// Create the purchase
        /// </summary>
        /// <param name="request">Purchase model</param>
        /// <returns>Result of create process</returns>
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


        /// <summary>
        /// Update the purchase by id
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <param name="request">Purchase model</param>
        /// <returns>Result of update process</returns>
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

        /// <summary>
        /// Get list purchase in the trash
        /// </summary>
        /// <returns>Return list of purchase in the trash</returns>
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

        /// <summary>
        /// Soft delete the purchase by id
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <returns>Result of soft delete process</returns>
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

        /// <summary>
        /// Restore purchase by id
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <returns>Result of restore process</returns>
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

        /// <summary>
        /// Permanent delete purchase by id
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <returns>Result of permanent process</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> PermanentDeletePurchase(int id)
        {
            _logger.LogInformation("Begin PermanentDeletePurchase API");

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
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

            foreach (var purchaseItem in purchaseItemInPurchase)
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

        #endregion

        #region PurchaseItems


        #endregion
    }
}
