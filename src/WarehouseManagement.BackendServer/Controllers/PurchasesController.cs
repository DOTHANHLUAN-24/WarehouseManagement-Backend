using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.PurchaseItems;
using WarehouseManagement.ViewModels.Contents.Purchases;
using WarehouseManagement.ViewModels.Systems;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Thêm Authorize để bắt buộc đăng nhập mới lấy được User ID
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
        /// <returns>Return list purchase with items</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPurchase()
        {
            _logger.LogInformation("Begin GetAllPurchase API");

            var purchases = await _context.Purchases
                .Where(p => !p.IsDeleted)
                .Include(p => p.PurchaseItems)
                .ToListAsync();

            var response = purchases.Select(x => new
            {
                x.Id,
                x.ReceiptCode,
                x.ReferenceCode,
                x.Note,
                x.SupplierId,
                x.SupplierName,
                x.PurchaseDate,
                TotalAmount = x.TotalCost,
                x.CreateDate,
                x.LastModifiedDate,
                Status = (int)x.Status,
                x.IsCanceled,
                Items = x.PurchaseItems.Where(i => !i.IsDeleted).Select(pi => new
                {
                    pi.ProductId,
                    pi.ProductVariantId,
                    pi.Quantity,
                    UnitCost = pi.UnitCost,
                    TotalPrice = pi.TotalPrice
                }).ToList()
            }).ToList();

            _logger.LogInformation("Success GetAllPurchase API");
            return Ok(response);
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

            var query = _context.Purchases.Where(p => !p.IsDeleted).AsQueryable();

            if (fromDate != null)
            {
                query = query.Where(x => x.CreateDate.Date >= fromDate.Value.Date);
            }

            if (toDate != null)
            {
                query = query.Where(x => x.CreateDate.Date <= toDate.Value.Date);
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .Include(p => p.PurchaseItems)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var data = items.Select(x => new
            {
                x.Id,
                x.ReceiptCode,
                x.ReferenceCode,
                x.Note,
                x.SupplierId,
                x.SupplierName,
                x.PurchaseDate,
                TotalAmount = x.TotalCost,
                x.CreateDate,
                x.LastModifiedDate,
                Status = (int)x.Status,
                x.IsCanceled,
                Items = x.PurchaseItems.Where(i => !i.IsDeleted).Select(pi => new
                {
                    pi.ProductId,
                    pi.ProductVariantId,
                    pi.Quantity,
                    UnitCost = pi.UnitCost,
                    TotalPrice = pi.TotalPrice
                }).ToList()
            }).ToList();

            var pagination = new
            {
                Items = data,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        /// <summary>
        /// Get purchase by id with items
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <returns>The purchase with items or not found</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseById(int id)
        {
            _logger.LogInformation("Begin GetPurchaseById API");

            var purchase = await _context.Purchases
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.PurchaseItems)
                .FirstOrDefaultAsync();

            if (purchase == null)
            {
                _logger.LogError("Not found the purchase with id = {id}", id);
                return NotFound();
            }

            var response = new
            {
                purchase.Id,
                purchase.ReceiptCode,
                purchase.ReferenceCode,
                purchase.Note,
                purchase.SupplierId,
                purchase.SupplierName,
                purchase.PurchaseDate,
                TotalAmount = purchase.TotalCost,
                CreateDate = purchase.CreateDate,
                LastModifiedDate = purchase.LastModifiedDate,
                Status = (int)purchase.Status,
                IsCanceled = purchase.IsCanceled,
                CancelReason = purchase.CancelReason,
                CanceledDate = purchase.CanceledDate,
                CanceledBy = purchase.CanceledBy,
                Items = purchase.PurchaseItems.Where(i => !i.IsDeleted).Select(x => new
                {
                    x.ProductId,
                    x.ProductVariantId,
                    x.Quantity,
                    UnitCost = x.UnitCost,
                    TotalPrice = x.TotalPrice
                }).ToList()
            };

            _logger.LogInformation("Return the purchase with id = {id}", id);
            return Ok(response);
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
            if (request == null)
            {
                _logger.LogWarning("PostPurchase called with null request");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for PostPurchase");
                return BadRequest(ModelState);
            }

            // Validate supplier
            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            if (supplier == null || supplier.IsDeleted)
            {
                _logger.LogWarning("Supplier not found or deleted. SupplierId = {SupplierId}", request.SupplierId);
                return BadRequest("Supplier not found");
            }

            if (request.Items == null || !request.Items.Any())
            {
                _logger.LogWarning("No items provided for purchase");
                return BadRequest("Purchase must contain at least one item");
            }

            // XỬ LÝ SỬA: Đồng bộ múi giờ Việt Nam để sinh chuỗi hiển thị 'yyyyMM' chính xác
            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var ym = localTime.ToString("yyyyMM");

            var sequence = await _context.Purchases.CountAsync(p => p.CreateDate.Year == utcNow.Year && p.CreateDate.Month == utcNow.Month) + 1;
            var receiptCode = $"PO-{ym}-{sequence:000}";

            // THÊM MỚI: Lấy thông tin User ID từ tài khoản đang đăng nhập
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var purchase = new Purchase
            {
                SupplierId = request.SupplierId,
                SupplierName = supplier.SupplierName,
                PurchaseDate = request.ReceiptDate == default ? request.PurchaseDate : request.ReceiptDate,
                ReceiptCode = receiptCode,
                ReferenceCode = request.ReferenceCode,
                Note = request.Note,
                CreateDate = utcNow,
                CreatedBy = currentUserId, // GÁN THÔNG TIN USER ĐĂNG NHẬP
                IsCanceled = false
            };

            decimal totalCost = 0m;

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                {
                    _logger.LogWarning("Invalid quantity for productId = {ProductId}", item.ProductId);
                    return BadRequest($"Invalid quantity for product {item.ProductId}");
                }

                // find a product variant for the given product id
                var variant = await _context.ProductVariants
                    .Where(v => v.ProductId == item.ProductId && v.IsActive)
                    .OrderBy(v => v.Id)
                    .FirstOrDefaultAsync();
                if (variant == null)
                {
                    _logger.LogWarning("No active variant found for productId = {ProductId}", item.ProductId);
                    return BadRequest($"No product variant found for product {item.ProductId}");
                }

                var pi = new PurchaseItem
                {
                    ProductVariantId = variant.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    CreateDate = utcNow,
                    TotalPrice = item.UnitCost * item.Quantity
                };
                purchase.PurchaseItems.Add(pi);

                totalCost += pi.TotalPrice ?? 0m;
            }

            purchase.TotalCost = totalCost;
            _context.Purchases.Add(purchase);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Success PostPurchase API with id = {id}", purchase.Id);

                    var response = new
                    {
                        purchase.Id,
                        purchase.ReceiptCode,
                        purchase.ReferenceCode,
                        purchase.Note,
                        purchase.SupplierId,
                        purchase.SupplierName,
                        purchase.PurchaseDate,
                        TotalAmount = purchase.TotalCost,
                        purchase.CreatedBy, // Trả thêm CreatedBy ở phản hồi tạo mới thành công
                        Items = purchase.PurchaseItems.Select(x => new
                        {
                            x.ProductId,
                            x.ProductVariantId,
                            x.Quantity,
                            UnitCost = x.UnitCost,
                            TotalPrice = x.TotalPrice
                        })
                    };
                    return CreatedAtAction(nameof(GetPurchaseById), new { id = purchase.Id }, response);
                }

                _logger.LogWarning("PostPurchase did not persist changes");
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating purchase");
                return StatusCode(500, "An error occurred while saving the purchase.");
            }
        }

        /// <summary>
        /// Update the purchase by id
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <param name="request">Purchase model</param>
        /// <returns>Result of update process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] PurchaseCreateRequest request)
        {
            _logger.LogInformation("Begin UpdatePurchase API");
            if (request == null)
            {
                _logger.LogWarning("UpdatePurchase called with null request. Id = {id}", id);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdatePurchase. Id = {id}", id);
                return BadRequest(ModelState);
            }

            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
            {
                _logger.LogWarning("Not found the purchase with id = {id}", id);
                return NotFound();
            }
            if (purchase.IsCanceled)
            {
                _logger.LogWarning("Attempt to update a canceled purchase. Id = {id}", id);
                return BadRequest("Cannot update a canceled purchase");
            }

            var supplier = await _context.Suppliers.FindAsync(request.SupplierId);
            if (supplier == null || supplier.IsDeleted)
            {
                _logger.LogWarning("Supplier not found for update. SupplierId = {SupplierId}", request.SupplierId);
                return BadRequest("Supplier not found");
            }

            // SỬA: Xóa bỏ hoàn toàn các PurchaseItem cũ khỏi danh sách đang theo dõi của Purchase
            // Việc này giải phóng bộ nhớ tracking của EF Core để tránh lỗi trùng Key
            purchase.PurchaseItems.Clear();

            purchase.SupplierId = request.SupplierId;
            purchase.SupplierName = supplier.SupplierName;
            purchase.PurchaseDate = request.ReceiptDate == default ? request.PurchaseDate : request.ReceiptDate;
            purchase.ReferenceCode = request.ReferenceCode;
            purchase.Note = request.Note;
            purchase.LastModifiedDate = DateTime.UtcNow;

            decimal totalCost = 0m;
            if (request.Items != null)
            {
                foreach (var item in request.Items)
                {
                    if (item.Quantity <= 0)
                    {
                        _logger.LogWarning("Invalid quantity for productId = {ProductId} during update", item.ProductId);
                        return BadRequest($"Invalid quantity for product {item.ProductId}");
                    }

                    var variant = await _context.ProductVariants
                        .Where(v => v.ProductId == item.ProductId && v.IsActive)
                        .OrderBy(v => v.Id)
                        .FirstOrDefaultAsync();

                    if (variant == null)
                    {
                        _logger.LogWarning("No active variant found for productId = {ProductId} during update", item.ProductId);
                        return BadRequest($"No product variant found for product {item.ProductId}");
                    }

                    var pi = new PurchaseItem
                    {
                        ProductVariantId = variant.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost,
                        CreateDate = DateTime.UtcNow,
                        TotalPrice = item.UnitCost * item.Quantity
                    };

                    // SỬA: Thêm trực tiếp vào danh sách của purchase thay vì gọi _context.PurchaseItems.Add(pi)
                    purchase.PurchaseItems.Add(pi);

                    totalCost += pi.TotalPrice ?? 0m;
                }
            }

            purchase.TotalCost = totalCost;
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Success to update the purchase with id = {id}", id);
                    return NoContent();
                }

                _logger.LogWarning("Fail to update purchase with id = {id}", id);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating purchase with id = {id}", id);
                return StatusCode(500, "An error occurred while updating the purchase.");
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
                _logger.LogWarning("Not found the purchase with id = {id}", id);
                return NotFound();
            }

            if (purchase.IsDeleted)
            {
                _logger.LogWarning("Purchase already in the trash. Id = {id}", id);
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

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("SoftDeletePurchase success. Id = {id}", id);
                    return Ok(result);
                }

                _logger.LogWarning("SoftDeletePurchase failed to save changes. Id = {id}", id);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during soft delete for purchase id = {id}", id);
                return StatusCode(500, "An error occurred while soft-deleting the purchase.");
            }
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
                _logger.LogWarning("Not found the purchase with id = {id}", id);
                return NotFound();
            }

            if (!purchase.IsDeleted)
            {
                _logger.LogWarning("Purchase not in trash. Purchase id = {id}", id);
                return BadRequest("Purchase is not in trash");
            }

            purchase.IsDeleted = false;
            var purchaseItemInPurchase = await _context.PurchaseItems
               .Where(x => x.PurchaseId == id)
               .ToListAsync();
            foreach (var purchaseItem in purchaseItemInPurchase)
            {
                purchaseItem.IsDeleted = false;
            }

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("RestorePurchase success. Purchase id = {id}", id);
                    return Ok(result);
                }

                _logger.LogWarning("RestorePurchase failed to save changes. Purchase id = {id}", id);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during restore for purchase id = {id}", id);
                return StatusCode(500, "An error occurred while restoring the purchase.");
            }
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
                _logger.LogWarning("Not found the purchase with id = {id}", id);
                return NotFound();
            }

            if (!purchase.IsDeleted)
            {
                _logger.LogWarning("Purchase must be soft-deleted before permanent deletion. Id = {id}", id);
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
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("PermanentDeletePurchase success. Id = {id}", id);
                    return NoContent();
                }

                _logger.LogWarning("PermanentDeletePurchase failed to save changes. Id = {id}", id);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during permanent delete for purchase id = {id}", id);
                return StatusCode(500, "An error occurred while permanently deleting the purchase.");
            }
        }

        /// <summary>
        /// Confirm the purchase and save into stock transactions        
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <returns>Result of process</returns>
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmPurchase(int id)
        {
            _logger.LogInformation("Begin ConfirmPurchase API");

            var existPurchase = await _context.Purchases.FindAsync(id);
            if (existPurchase == null)
            {
                _logger.LogWarning("Can't found the purchase with id = {id}", id);
                return NotFound();
            }

            var listPurchaseItem = await _context.PurchaseItems
                .Where(x => x.PurchaseId == id && !x.IsDeleted)
                .ToListAsync();
            if (listPurchaseItem == null || listPurchaseItem.Count == 0)
            {
                _logger.LogWarning("No purchase items found to confirm for purchase id = {id}", id);
                return BadRequest("No purchase items to confirm");
            }

            var totalPrice = 0;
            foreach (var item in listPurchaseItem)
            {
                var price = ((int)item.UnitCost * item.Quantity);
                totalPrice += price;
                var product = await _context.ProductVariants
                    .Where(x => x.Id == item.ProductVariantId)
                    .FirstOrDefaultAsync();
                if (product == null)
                {
                    _logger.LogWarning(
                        "ProductVariant not found. ProductVariantId = {productVariantId} for purchase id = {purchaseId}",
                        item.ProductVariantId,
                        id
                    );
                    return BadRequest($"ProductVariant not found. Id = {item.ProductVariantId}");
                }

                var stockTransaction = new StockTransaction
                {
                    ProductId = product.ProductId,
                    ReferenceId = id,
                    ReferenceType = Data.Enums.ReferenceType.Purchase,
                    WarehouseId = 1, // Fake
                    QuantityChange = item.Quantity,
                    BalanceAfter = price,
                    CreateDate = DateTime.UtcNow,
                };

                _context.StockTransactions.Add(stockTransaction);
            }

            if (totalPrice != existPurchase.TotalCost)
            {
                _logger.LogInformation("Change total price with new total price = {totalPrice}", totalPrice);
                existPurchase.TotalCost = totalPrice;
            }

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Success ConfirmPurchase API for id = {id}", id);
                    return Ok();
                }

                _logger.LogWarning("Failed to save changes during ConfirmPurchase for id = {id}", id);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while confirming purchase id = {id}", id);
                return StatusCode(500, "An error occurred while confirming the purchase.");
            }
        }

        #endregion

        #region PurchaseItems

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetItemsByPurchaseId(int id)
        {
            var listPurchaseItems = await _context.PurchaseItems
                .Where(x => x.PurchaseId == id && !x.IsDeleted)
                .ToListAsync();

            return Ok(listPurchaseItems);
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItemToPurchase(int id, PurchaseItemCreateRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("AddItemToPurchase called with null request. PurchaseId = {id}", id);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddItemToPurchase. PurchaseId = {id}", id);
                return BadRequest(ModelState);
            }

            var existPurchase = await _context.Purchases.FindAsync(id);
            if (existPurchase == null)
                return NotFound();

            var purchaseItem = new PurchaseItem
            {
                PurchaseId = id,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity,
                UnitCost = request.UnitCost,
                CreateDate = request.CreateDate,
                LastModifiedDate = request.LastModifiedDate,
            };
            _context.PurchaseItems.Add(purchaseItem);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return CreatedAtAction(nameof(GetItemsByPurchaseId), new { id = id }, purchaseItem);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding purchase item to purchase id = {id}", id);
                return StatusCode(500, "An error occurred while adding the purchase item.");
            }
        }

        [HttpDelete("{id}/items/{itemId}/soft-delete")]
        public async Task<IActionResult> SoftDeletePurchaseItem(int id, int itemId)
        {
            var existPurchase = await _context.Purchases.FindAsync(id);
            if (existPurchase == null)
                return NotFound();

            var existPurchaseItem = await _context.PurchaseItems.FindAsync(itemId);
            if (existPurchaseItem == null)
                return NotFound();

            if (existPurchase.IsDeleted)
                return BadRequest("Purchase is deleted");
            if (existPurchaseItem.PurchaseId != id)
                return BadRequest("Item does not belong to the given purchase");
            if (existPurchaseItem.IsDeleted)
                return BadRequest("Item already deleted");

            existPurchaseItem.IsDeleted = true;

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return Ok(existPurchaseItem);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while soft-deleting purchase item id = {itemId} for purchase id = {id}", itemId, id);
                return StatusCode(500, "An error occurred while soft-deleting the purchase item.");
            }
        }

        [HttpPut("{id}/items/{itemId}/restore")]
        public async Task<IActionResult> RestorePurchaseItem(int id, int itemId)
        {
            var existPurchase = await _context.Purchases.FindAsync(id);
            if (existPurchase == null)
                return NotFound();

            var existPurchaseItem = await _context.PurchaseItems.FindAsync(itemId);
            if (existPurchaseItem == null)
                return NotFound();

            if (existPurchaseItem.PurchaseId != id)
                return BadRequest("Item does not belong to the given purchase");
            if (!existPurchaseItem.IsDeleted)
                return BadRequest("Item is not deleted");

            existPurchaseItem.IsDeleted = false;

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return Ok(existPurchaseItem);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while restoring purchase item id = {itemId} for purchase id = {id}", itemId, id);
                return StatusCode(500, "An error occurred while restoring the purchase item.");
            }
        }

        [HttpDelete("{id}/items/{itemId}/permanent-delete")]
        public async Task<IActionResult> PermanentDeletePurchaseItem(int id, int itemId)
        {
            var existPurchase = await _context.Purchases.FindAsync(id);
            if (existPurchase == null)
                return NotFound();

            var existPurchaseItem = await _context.PurchaseItems.FindAsync(itemId);
            if (existPurchaseItem == null)
                return NotFound();

            if (existPurchaseItem.PurchaseId != id)
                return BadRequest("Item does not belong to the given purchase");
            if (!existPurchaseItem.IsDeleted)
                return BadRequest("Item must be soft-deleted before permanent deletion");

            _context.PurchaseItems.Remove(existPurchaseItem);

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return Ok(existPurchaseItem);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while permanently deleting purchase item id = {itemId} for purchase id = {id}", itemId, id);
                return StatusCode(500, "An error occurred while permanently deleting the purchase item.");
            }
        }

        #endregion
    }
}