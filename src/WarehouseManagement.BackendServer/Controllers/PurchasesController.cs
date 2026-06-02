using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClosedXML.Excel;
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
                x.CustomerId,
                x.CustomerName,
                x.IsExport,
                x.Type,
                x.PurchaseDate,
                TotalAmount = x.TotalCost,
                x.CreateDate,
                x.LastModifiedDate,
                Status = (int)x.Status,
                x.IsCanceled,
                NoteCancel = x.Status == Data.Enums.PurchaseStatus.Canceled ? x.NoteCancel : null,
                Items = x.PurchaseItems.Where(i => !i.IsDeleted).Select(pi => new
                {
                    pi.ProductId,
                    pi.ProductVariantId,
                    pi.Quantity,
                    UnitCost = pi.UnitCost,
                    TotalPrice = pi.TotalPrice,
                    WarehouseLocation = pi.WarehouseLocation
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
            Data.Enums.PurchaseStatus? status,
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Purchases.Where(p => !p.IsDeleted).AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

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
                x.CustomerId,
                x.CustomerName,
                x.IsExport,
                x.Type,
                x.PurchaseDate,
                TotalAmount = x.TotalCost,
                x.CreateDate,
                x.LastModifiedDate,
                Status = (int)x.Status,
                x.IsCanceled,
                NoteCancel = x.Status == Data.Enums.PurchaseStatus.Canceled ? x.NoteCancel : null,
                Items = x.PurchaseItems.Where(i => !i.IsDeleted).Select(pi => new
                {
                    pi.ProductId,
                    pi.ProductVariantId,
                    pi.Quantity,
                    UnitCost = pi.UnitCost,
                    TotalPrice = pi.TotalPrice,
                    WarehouseLocation = pi.WarehouseLocation
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
        /// Export purchases to Excel file
        /// </summary>
        /// <param name="fromDate">Start date for filter</param>
        /// <param name="toDate">End date for filter</param>
        /// <returns>Excel file with purchase data</returns>
        [HttpGet("export")]
        public async Task<IActionResult> ExportPurchasesToExcel(DateTime? fromDate, DateTime? toDate, Data.Enums.PurchaseStatus? status)
        {
            _logger.LogInformation("Begin ExportPurchasesToExcel API. FromDate={FromDate}, ToDate={ToDate}, Status={Status}", fromDate, toDate, status);
            var query = _context.Purchases
                .Where(p => !p.IsDeleted)
                .Include(p => p.PurchaseItems)
                .AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (fromDate != null)
            {
                query = query.Where(x => x.CreateDate.Date >= fromDate.Value.Date);
            }

            if (toDate != null)
            {
                query = query.Where(x => x.CreateDate.Date <= toDate.Value.Date);
            }

            var purchases = await query.OrderByDescending(x => x.CreateDate).ToListAsync();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Purchases");
                worksheet.Cell(1, 1).Value = "Receipt Code";
                worksheet.Cell(1, 2).Value = "Reference Code";
                worksheet.Cell(1, 3).Value = "Supplier Name";
                worksheet.Cell(1, 4).Value = "Purchase Date";
                worksheet.Cell(1, 5).Value = "Total Cost";
                worksheet.Cell(1, 6).Value = "Status";
                worksheet.Cell(1, 7).Value = "Created Date";
                worksheet.Cell(1, 8).Value = "Items Count";
                worksheet.Cell(1, 9).Value = "Items Details";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRow.Style.Font.Bold = true;

                int row = 2;
                foreach (var purchase in purchases)
                {
                    var itemsCount = purchase.PurchaseItems.Count(i => !i.IsDeleted);
                    var itemsDetails = string.Join("; ", purchase.PurchaseItems
                        .Where(i => !i.IsDeleted)
                        .Select(i => $"ProductVariantId: {i.ProductVariantId}, Qty: {i.Quantity}, Cost: {i.UnitCost}"));
                    worksheet.Cell(row, 1).Value = purchase.ReceiptCode;
                    worksheet.Cell(row, 2).Value = purchase.ReferenceCode;
                    worksheet.Cell(row, 3).Value = purchase.SupplierName;
                    worksheet.Cell(row, 4).Value = purchase.PurchaseDate?.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 5).Value = purchase.TotalCost;
                    worksheet.Cell(row, 6).Value = purchase.Status.ToString();
                    worksheet.Cell(row, 7).Value = purchase.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cell(row, 8).Value = itemsCount;
                    worksheet.Cell(row, 9).Value = itemsDetails;

                    row++;
                }

                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileBytes = stream.ToArray();

                    var fileName = $"Purchases_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    _logger.LogInformation("ExportPurchasesToExcel success. FileName={FileName}", fileName);

                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
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
                purchase.CustomerId,
                purchase.CustomerName,
                purchase.IsExport,
                purchase.Type,
                purchase.PurchaseDate,
                TotalAmount = purchase.TotalCost,
                CreateDate = purchase.CreateDate,
                LastModifiedDate = purchase.LastModifiedDate,
                Status = (int)purchase.Status,
                IsCanceled = purchase.IsCanceled,
                NoteCancel = purchase.Status == Data.Enums.PurchaseStatus.Canceled ? purchase.NoteCancel : null,
                purchase.CanceledDate,
                purchase.CanceledBy,
                purchase.CreatedBy,
                purchase.ApprovedBy,
                purchase.ApprovedDate,
                Items = purchase.PurchaseItems.Where(i => !i.IsDeleted).Select(x => new
                {
                    x.ProductId,
                    x.ProductVariantId,
                    x.Quantity,
                    UnitCost = x.UnitCost,
                    TotalPrice = x.TotalPrice,
                    WarehouseLocation = x.WarehouseLocation
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

            string? supplierName = null;
            string? customerName = null;
            bool isExport = request.Type == 2;

            if (isExport)
            {
                if (!request.CustomerId.HasValue)
                {
                    return BadRequest("CustomerId is required for export voucher");
                }
                var customer = await _context.Customers.FindAsync(request.CustomerId.Value);
                if (customer == null || customer.IsDeleted)
                {
                    _logger.LogWarning("Customer not found or deleted. CustomerId = {CustomerId}", request.CustomerId);
                    return BadRequest("Customer not found");
                }
                customerName = customer.FullName;
            }
            else
            {
                if (!request.SupplierId.HasValue)
                {
                    return BadRequest("SupplierId is required for import voucher");
                }
                var supplier = await _context.Suppliers.FindAsync(request.SupplierId.Value);
                if (supplier == null || supplier.IsDeleted)
                {
                    _logger.LogWarning("Supplier not found or deleted. SupplierId = {SupplierId}", request.SupplierId);
                    return BadRequest("Supplier not found");
                }
                supplierName = supplier.SupplierName;
            }

            if (request.Items == null || !request.Items.Any())
            {
                _logger.LogWarning("No items provided for purchase");
                return BadRequest("Purchase must contain at least one item");
            }

            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var prefix = isExport ? "SO" : "PO";
            var datePart = localTime.ToString("yyyyMMdd");
            var lastCode = await _context.Purchases
                .Where(p => p.ReceiptCode.StartsWith($"{prefix}-{datePart}"))
                .OrderByDescending(p => p.ReceiptCode)
                .Select(p => p.ReceiptCode)
                .FirstOrDefaultAsync();
            int sequence = 1;

            if (lastCode != null)
            {
                sequence = int.Parse(lastCode.Split('-')[2]) + 1;
            }

            var receiptCode = $"{prefix}-{datePart}-{sequence:000}";

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var purchase = new Purchase
            {
                SupplierId = isExport ? null : request.SupplierId,
                SupplierName = supplierName,
                CustomerId = isExport ? request.CustomerId : null,
                CustomerName = customerName,
                IsExport = isExport,
                Type = request.Type,
                PurchaseDate = request.ReceiptDate == default ? request.PurchaseDate : request.ReceiptDate,
                ReceiptCode = receiptCode,
                ReferenceCode = request.ReferenceCode,
                Note = request.Note,
                CreateDate = utcNow,
                Status = Data.Enums.PurchaseStatus.None,
                CreatedBy = currentUserId,
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
                    WarehouseLocation = item.WarehouseLocation,
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
                        purchase.CustomerId,
                        purchase.CustomerName,
                        purchase.IsExport,
                        purchase.Type,
                        purchase.PurchaseDate,
                        TotalAmount = purchase.TotalCost,
                        purchase.CreatedBy,
                        Items = purchase.PurchaseItems.Select(x => new
                        {
                            x.ProductId,
                            x.ProductVariantId,
                            x.Quantity,
                            UnitCost = x.UnitCost,
                            TotalPrice = x.TotalPrice,
                            WarehouseLocation = x.WarehouseLocation
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
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] PurchaseUpdateRequest request)
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
            if (purchase.Status == Data.Enums.PurchaseStatus.Completed || purchase.ApprovedDate != null)
            {
                _logger.LogWarning("Attempt to update an approved/completed purchase. Id = {id}", id);
                return BadRequest("Cannot update an approved/completed purchase");
            }

            string? supplierName = null;
            string? customerName = null;
            bool isExport = request.Type == 2;
            if (isExport)
            {
                if (!request.CustomerId.HasValue)
                {
                    return BadRequest("CustomerId is required for export voucher");
                }
                var customer = await _context.Customers.FindAsync(request.CustomerId.Value);
                if (customer == null || customer.IsDeleted)
                {
                    _logger.LogWarning("Customer not found or deleted. CustomerId = {CustomerId}", request.CustomerId);
                    return BadRequest("Customer not found");
                }
                customerName = customer.FullName;
            }
            else
            {
                if (!request.SupplierId.HasValue)
                {
                    return BadRequest("SupplierId is required for import voucher");
                }
                var supplier = await _context.Suppliers.FindAsync(request.SupplierId.Value);
                if (supplier == null || supplier.IsDeleted)
                {
                    _logger.LogWarning("Supplier not found or deleted. SupplierId = {SupplierId}", request.SupplierId);
                    return BadRequest("Supplier not found");
                }
                supplierName = supplier.SupplierName;
            }

            // Synchronize ReceiptCode prefix if the type changed
            if (purchase.IsExport != isExport && !string.IsNullOrEmpty(purchase.ReceiptCode))
            {
                var newPrefix = isExport ? "SO" : "PO";
                var oldPrefix = purchase.IsExport ? "SO" : "PO";
                if (purchase.ReceiptCode.StartsWith(oldPrefix))
                {
                    purchase.ReceiptCode = newPrefix + purchase.ReceiptCode.Substring(oldPrefix.Length);
                }
                else
                {
                    purchase.ReceiptCode = purchase.ReceiptCode.Replace(oldPrefix, newPrefix);
                }
            }

            // SỬA: Xóa bỏ hoàn toàn các PurchaseItem cũ khỏi danh sách đang theo dõi của Purchase
            // Việc này giải phóng bộ nhớ tracking của EF Core để tránh lỗi trùng Key
            purchase.PurchaseItems.Clear();
            purchase.SupplierId = isExport ? null : request.SupplierId;
            purchase.SupplierName = supplierName;
            purchase.CustomerId = isExport ? request.CustomerId : null;
            purchase.CustomerName = customerName;
            purchase.IsExport = isExport;
            purchase.Type = request.Type;
            purchase.PurchaseDate = request.ReceiptDate == default ? request.PurchaseDate : request.ReceiptDate;
            purchase.ReferenceCode = request.ReferenceCode;
            purchase.Note = request.Note;
            purchase.Status = (Data.Enums.PurchaseStatus)request.Status;
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
                        WarehouseLocation = item.WarehouseLocation,
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
            purchase.LastModifiedDate = DateTime.UtcNow;
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
            purchase.LastModifiedDate = DateTime.UtcNow;
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

            if (existPurchase.IsDeleted)
            {
                return BadRequest("Purchase is deleted");
            }
            if (existPurchase.IsCanceled || existPurchase.Status == Data.Enums.PurchaseStatus.Canceled)
            {
                return BadRequest("Cannot confirm a canceled purchase");
            }
            if (existPurchase.Status == Data.Enums.PurchaseStatus.Completed || existPurchase.ApprovedDate != null)
            {
                return BadRequest("Cannot confirm an already completed purchase");
            }
            if (existPurchase.Status != Data.Enums.PurchaseStatus.None)
            {
                return BadRequest("Phiếu phải ở trạng thái Mới tạo (None/0) để xác nhận duyệt.");
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
            }

            if (totalPrice != existPurchase.TotalCost)
            {
                _logger.LogInformation("Change total price with new total price = {totalPrice}", totalPrice);
                existPurchase.TotalCost = totalPrice;
            }

            // Khi xác nhận (confirm) => chuyển trạng thái từ None (0) sang Pending (1)
            // Không gán ApprovedBy/ApprovedDate ở bước confirm; bước approve mới gán
            existPurchase.Status = Data.Enums.PurchaseStatus.Pending;
            existPurchase.LastModifiedDate = DateTime.UtcNow;

            try
            {
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation("Success ConfirmPurchase API for id = {id}", id);
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while confirming purchase id = {id}", id);
                return StatusCode(500, "An error occurred while confirming the purchase.");
            }
        }

        /// <summary>
        /// Cancel the purchase by id (mark as canceled) and save the user who cancelled
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelPurchase(int id, [FromBody] WarehouseManagement.ViewModels.Contents.Purchases.PurchaseCancelRequest? request)
        {
            _logger.LogInformation("Begin CancelPurchase API");

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                _logger.LogWarning("Cannot find the purchase with id = {id} to cancel", id);
                return NotFound();
            }

            if (purchase.IsCanceled)
            {
                _logger.LogWarning("Purchase already canceled. Id = {id}", id);
                return BadRequest("Purchase is already canceled");
            }

            // Only Pending purchases can be canceled (same rule as approve requires Pending)
            if (purchase.Status != Data.Enums.PurchaseStatus.Pending)
            {
                _logger.LogWarning("Purchase must be in Pending state to cancel. Id = {id}, Status = {status}", id, purchase.Status);
                return BadRequest("Purchase must be in Pending status to cancel.");
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            purchase.IsCanceled = true;
            // entity stores cancel note in NoteCancel property
            purchase.NoteCancel = request?.cancelReason;
            purchase.CanceledBy = currentUserId;
            purchase.CanceledDate = DateTime.UtcNow;
            purchase.LastModifiedDate = DateTime.UtcNow;
            purchase.Status = Data.Enums.PurchaseStatus.Canceled;
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("CancelPurchase success. Id = {id}", id);
                    return Ok();
                }

                _logger.LogWarning("CancelPurchase failed to save changes. Id = {id}", id);
                return BadRequest();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while canceling purchase id = {id}", id);
                return StatusCode(500, "An error occurred while canceling the purchase.");
            }
        }

        /// <summary>
        /// Approve the purchase by id
        /// </summary>
        /// <param name="id">Purchase id</param>
        /// <returns>Result of approve process</returns>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApprovePurchase(int id)
        {
            _logger.LogInformation("Begin ApprovePurchase API");

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                _logger.LogWarning("Cannot find the purchase with id = {id}", id);
                return NotFound();
            }

            if (purchase.IsCanceled || purchase.Status == Data.Enums.PurchaseStatus.Canceled)
            {
                _logger.LogWarning("Cannot approve a canceled purchase. Id = {id}", id);
                return BadRequest("Cannot approve a canceled purchase");
            }

            if (purchase.Status != Data.Enums.PurchaseStatus.Pending)
            {
                _logger.LogWarning("Purchase is not in Pending status. Id = {id}, Status = {status}", id, purchase.Status);
                return BadRequest("Phiếu phải ở trạng thái Chờ duyệt (Pending/1) mới có thể phê duyệt.");
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            purchase.ApprovedBy = currentUserId;
            purchase.ApprovedDate = DateTime.UtcNow;
            purchase.LastModifiedDate = DateTime.UtcNow;
            purchase.Status = Data.Enums.PurchaseStatus.Completed; // status = 2

            var listPurchaseItems = await _context.PurchaseItems
                .Where(x => x.PurchaseId == id && !x.IsDeleted)
                .ToListAsync();
            if (listPurchaseItems == null || listPurchaseItems.Count == 0)
            {
                return BadRequest("Phiếu không có sản phẩm nào để duyệt.");
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in listPurchaseItems)
                {
                    var productVariant = await _context.ProductVariants.FindAsync(item.ProductVariantId);
                    if (productVariant == null)
                    {
                        return BadRequest($"Không tìm thấy biến thể sản phẩm ID: {item.ProductVariantId}");
                    }

                    int actualQuantityChange = purchase.IsExport ? -item.Quantity : item.Quantity;

                    if (purchase.IsExport && productVariant.StockQuantity + actualQuantityChange < 0)
                    {
                        return BadRequest($"Sản phẩm (ID biến thể: {productVariant.Id}) không đủ tồn kho để xuất. Hiện có: {productVariant.StockQuantity}, yêu cầu xuất: {item.Quantity}");
                    }

                    productVariant.StockQuantity += actualQuantityChange;
                    _context.ProductVariants.Update(productVariant);

                    var lastBalance = await _context.StockTransactions
                        .Where(x => x.ProductId == productVariant.ProductId && x.WarehouseId == 1)
                        .OrderByDescending(x => x.Id)
                        .Select(x => x.BalanceAfter)
                        .FirstOrDefaultAsync();
                    var stockTransaction = new StockTransaction
                    {
                        ProductId = productVariant.ProductId,
                        ProductVariantId = productVariant.Id,
                        WarehouseId = 1,
                        QuantityChange = actualQuantityChange,
                        BalanceAfter = lastBalance + actualQuantityChange,
                        TransactionType = purchase.IsExport
                            ? Data.Enums.StockTransactionType.SalesIssue
                            : Data.Enums.StockTransactionType.PurchaseReceipt,
                        ReferenceType = purchase.IsExport
                            ? Data.Enums.ReferenceType.Order
                            : Data.Enums.ReferenceType.Purchase,
                        ReferenceId = id,
                        Note = purchase.Note ?? (purchase.IsExport ? "Xuất kho bán hàng" : "Nhập kho mua hàng"),
                        CreateDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        IsCanceled = false
                    };

                    _context.StockTransactions.Add(stockTransaction);
                }

                var result = await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                if (result > 0)
                {
                    _logger.LogInformation("ApprovePurchase success. Id = {id}", id);
                    return Ok(new { Message = "Phê duyệt phiếu thành công, kho đã được cập nhật.", Status = 2 });
                }

                _logger.LogWarning("ApprovePurchase failed to save changes. Id = {id}", id);
                return BadRequest("Không thể lưu thay đổi khi phê duyệt phiếu.");
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, "Database error while approving purchase id = {id}", id);
                return StatusCode(500, "An error occurred while approving the purchase.");
            }
        }

        /// <summary>
        /// Convert/switch the type of a purchase (Import <-> Export)
        /// </summary>
        [HttpPut("{id}/convert-type")]
        public async Task<IActionResult> ConvertPurchaseType(int id, [FromBody] PurchaseConvertTypeRequest request)
        {
            _logger.LogInformation("Begin ConvertPurchaseType API for id = {id}", id);
            if (request == null)
            {
                _logger.LogWarning("ConvertPurchaseType called with null request. Id = {id}", id);
                return BadRequest();
            }

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                _logger.LogWarning("Not found the purchase with id = {id}", id);
                return NotFound();
            }

            if (purchase.IsDeleted)
            {
                _logger.LogWarning("Cannot convert type of a deleted purchase. Id = {id}", id);
                return BadRequest("Cannot convert type of a deleted purchase");
            }

            if (purchase.IsCanceled)
            {
                _logger.LogWarning("Cannot convert type of a canceled purchase. Id = {id}", id);
                return BadRequest("Cannot convert type of a canceled purchase");
            }

            if (purchase.Status == Data.Enums.PurchaseStatus.Completed || purchase.ApprovedDate != null)
            {
                _logger.LogWarning("Cannot convert type of an approved/completed purchase. Id = {id}", id);
                return BadRequest("Cannot convert type of an approved/completed purchase");
            }

            bool currentIsExport = purchase.IsExport;
            bool targetIsExport = !currentIsExport;
            int targetType = targetIsExport ? 2 : 1;
            if (targetIsExport)
            {
                if (!request.CustomerId.HasValue)
                {
                    return BadRequest("CustomerId is required when converting to export");
                }
                var customer = await _context.Customers.FindAsync(request.CustomerId.Value);
                if (customer == null || customer.IsDeleted)
                {
                    _logger.LogWarning("Customer not found or deleted. CustomerId = {CustomerId}", request.CustomerId);
                    return BadRequest("Customer not found");
                }

                purchase.CustomerId = request.CustomerId;
                purchase.CustomerName = customer.FullName;
                purchase.SupplierId = null;
                purchase.SupplierName = null;
            }
            else
            {
                if (!request.SupplierId.HasValue)
                {
                    return BadRequest("SupplierId is required when converting to import");
                }
                var supplier = await _context.Suppliers.FindAsync(request.SupplierId.Value);
                if (supplier == null || supplier.IsDeleted)
                {
                    _logger.LogWarning("Supplier not found or deleted. SupplierId = {SupplierId}", request.SupplierId);
                    return BadRequest("Supplier not found");
                }

                purchase.SupplierId = request.SupplierId;
                purchase.SupplierName = supplier.SupplierName;
                purchase.CustomerId = null;
                purchase.CustomerName = null;
            }

            purchase.IsExport = targetIsExport;
            purchase.Type = targetType;
            purchase.LastModifiedDate = DateTime.UtcNow;

            // Synchronize ReceiptCode prefix (e.g. PO-xxxxxx to SO-xxxxxx and vice versa)
            if (!string.IsNullOrEmpty(purchase.ReceiptCode))
            {
                var newPrefix = targetIsExport ? "SO" : "PO";
                var oldPrefix = currentIsExport ? "SO" : "PO";
                if (purchase.ReceiptCode.StartsWith(oldPrefix))
                {
                    purchase.ReceiptCode = newPrefix + purchase.ReceiptCode.Substring(oldPrefix.Length);
                }
                else
                {
                    purchase.ReceiptCode = purchase.ReceiptCode.Replace(oldPrefix, newPrefix);
                }
            }

            try
            {
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation("Success to convert type of purchase with id = {id} to {targetType}", id, targetType);
                return Ok(new
                {
                    purchase.Id,
                    purchase.ReceiptCode,
                    purchase.Type,
                    purchase.IsExport,
                    purchase.SupplierId,
                    purchase.SupplierName,
                    purchase.CustomerId,
                    purchase.CustomerName,
                    purchase.LastModifiedDate
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while converting type of purchase with id = {id}", id);
                return StatusCode(500, "An error occurred while converting the purchase type.");
            }
        }

        // Duplicate cancel endpoint removed. Use the body-based CancelPurchase([FromBody] PurchaseCancelRequest) method above.

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
                WarehouseLocation = request.WarehouseLocation,
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