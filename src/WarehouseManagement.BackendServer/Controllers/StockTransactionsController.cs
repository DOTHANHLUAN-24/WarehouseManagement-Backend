using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.ViewModels.Contents.StockTransactions;
using WarehouseManagement.ViewModels.Enums;
using WarehouseManagement.ViewModels.Systems;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransactionsController
        (
            ApplicationDbContext _context,
            ILogger<StockTransactionsController> _logger
        ) : BaseController
    {
        /// <summary>
        /// Get all stock transactions in the system
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllStockTransactions()
        {
            _logger.LogInformation("Begin GetAllStockTransactions API");

            var listStockTransactions = await _context.StockTransactions
                .Select(x => CreateStockTransactionViewModel(x))
                .ToListAsync();

            _logger.LogInformation("GetAllStockTransactions success.");

            return Ok(listStockTransactions);
        }

        /// <summary>
        /// Get stock transaction by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockTransactionById(int id)
        {
            _logger.LogInformation("Begin GetStockTransactionById API");

            var stockTransaction = await _context.StockTransactions
                .Where(x => x.Id == id)
                .Select(x => CreateStockTransactionViewModel(x))
                .FirstOrDefaultAsync();
            if (stockTransaction == null)
            {
                _logger.LogError("Not found the stock transaction by id = {id}", id);
                return NotFound();
            }

            _logger.LogInformation("GetStockTransactionById success, id = {id}", id);

            return Ok(stockTransaction);
        }

        /// <summary>
        /// Create stock transactions with create model
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostStockTransaction([FromBody] StockTransactionCreateRequest request)
        {
            _logger.LogInformation("Begin PostStockTransaction API");

            var stockTransaction = new StockTransaction
            {
                ProductId = request.ProductId,
                ProductVariantId = request.ProductVariantId,
                WarehouseId = request.WarehouseId,
                QuantityChange = request.QuantityChange,
                TransactionType = (Data.Enums.StockTransactionType)request.TransactionType,
                ReferenceType = (Data.Enums.ReferenceType)request.ReferenceType,
                ReferenceId = request.ReferenceId,
                Note = request.Note,
                CreateDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                IsCanceled = false
            };

            var lastBalance = await _context.StockTransactions
                .Where(x => x.ProductId == request.ProductId && x.WarehouseId == request.WarehouseId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.BalanceAfter)
                .FirstOrDefaultAsync();

            if (request.QuantityChange == 0)
                return BadRequest("QuantityChange must not be 0");

            stockTransaction.BalanceAfter = lastBalance + request.QuantityChange;

            _context.StockTransactions.Add(stockTransaction);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("PostStockTransaction success with id = {id}", stockTransaction.Id);
                return CreatedAtAction(nameof(GetStockTransactionById), new { id = stockTransaction.Id }, stockTransaction);
            }

            _logger.LogWarning("PostStockTransaction failed to save changes");
            return BadRequest();
        }

        /// <summary>
        /// Nhập kho (Import stock)
        /// </summary>
        [HttpPost("import-stock")]
        public async Task<IActionResult> ImportStock([FromBody] StockTransactionCreateRequest request)
        {
            _logger.LogInformation("Begin ImportStock API");

            if (request.QuantityChange <= 0)
                return BadRequest("QuantityChange phải lớn hơn 0 khi thực hiện nhập kho.");

            var stockTransaction = new StockTransaction
            {
                ProductId = request.ProductId,
                ProductVariantId = request.ProductVariantId,
                WarehouseId = request.WarehouseId,
                QuantityChange = request.QuantityChange,
                TransactionType = (Data.Enums.StockTransactionType)request.TransactionType,
                ReferenceType = (Data.Enums.ReferenceType)request.ReferenceType,
                ReferenceId = request.ReferenceId,
                Note = request.Note,
                CreateDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                IsCanceled = false
            };

            var lastBalance = await _context.StockTransactions
                .Where(x => x.ProductId == request.ProductId && x.WarehouseId == request.WarehouseId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.BalanceAfter)
                .FirstOrDefaultAsync();

            stockTransaction.BalanceAfter = lastBalance + request.QuantityChange;

            _context.StockTransactions.Add(stockTransaction);

            // Cập nhật số lượng tồn kho thực tế trong ProductVariant
            var productVariant = await _context.ProductVariants.FindAsync(request.ProductVariantId);
            if (productVariant != null)
            {
                productVariant.StockQuantity += request.QuantityChange;
                _context.ProductVariants.Update(productVariant);
            }

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("ImportStock success with id = {id}", stockTransaction.Id);
                return Ok(CreateStockTransactionViewModel(stockTransaction));
            }

            _logger.LogWarning("ImportStock failed to save changes");
            return BadRequest("Lỗi khi lưu dữ liệu nhập kho.");
        }

        /// <summary>
        /// Xuất kho (Export stock)
        /// </summary>
        [HttpPost("export-stock")]
        public async Task<IActionResult> ExportStock([FromBody] StockTransactionCreateRequest request)
        {
            _logger.LogInformation("Begin ExportStock API");

            if (request.QuantityChange >= 0)
                return BadRequest("QuantityChange phải nhỏ hơn 0 khi thực hiện xuất kho (ví dụ: -5).");

            var lastBalance = await _context.StockTransactions
                .Where(x => x.ProductId == request.ProductId && x.WarehouseId == request.WarehouseId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.BalanceAfter)
                .FirstOrDefaultAsync();

            if (lastBalance + request.QuantityChange < 0)
                return BadRequest("Số lượng tồn kho hiện tại không đủ để xuất.");

            var stockTransaction = new StockTransaction
            {
                ProductId = request.ProductId,
                ProductVariantId = request.ProductVariantId,
                WarehouseId = request.WarehouseId,
                QuantityChange = request.QuantityChange,
                TransactionType = (Data.Enums.StockTransactionType)request.TransactionType,
                ReferenceType = (Data.Enums.ReferenceType)request.ReferenceType,
                ReferenceId = request.ReferenceId,
                Note = request.Note,
                CreateDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                BalanceAfter = lastBalance + request.QuantityChange,
                IsCanceled = false
            };

            _context.StockTransactions.Add(stockTransaction);

            // Cập nhật số lượng tồn kho thực tế trong ProductVariant
            var productVariant = await _context.ProductVariants.FindAsync(request.ProductVariantId);
            if (productVariant != null)
            {
                productVariant.StockQuantity += request.QuantityChange; // Vì QuantityChange đang âm nên dùng dấu +
                _context.ProductVariants.Update(productVariant);
            }

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("ExportStock success with id = {id}", stockTransaction.Id);
                return Ok(CreateStockTransactionViewModel(stockTransaction));
            }

            _logger.LogWarning("ExportStock failed to save changes");
            return BadRequest("Lỗi khi lưu dữ liệu xuất kho.");
        }

        /// <summary>
        /// Hủy giao dịch kho (Hoàn tác số lượng) bằng LINQ Join
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelStockTransaction(int id, [FromBody] StockTransactionCancelRequest request)
        {
            _logger.LogInformation("Begin CancelStockTransaction API with id = {id}", id);

            // Dùng LINQ Join để lấy StockTransaction và ProductVariant cùng lúc trong 1 Query
            var query = from st in _context.StockTransactions
                        join pv in _context.ProductVariants on st.ProductVariantId equals pv.Id
                        where st.Id == id
                        select new { StockTransaction = st, ProductVariant = pv };

            var data = await query.FirstOrDefaultAsync();

            if (data == null)
                return NotFound($"Không tìm thấy giao dịch kho có ID = {id} hoặc biến thể sản phẩm không tồn tại.");

            var transactionToCancel = data.StockTransaction;
            var productVariant = data.ProductVariant;

            if (transactionToCancel.IsCanceled)
                return BadRequest("Giao dịch này đã được hủy trước đó.");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Cập nhật trạng thái hủy cho StockTransaction
                transactionToCancel.IsCanceled = true;
                transactionToCancel.CancelReason = request.CancelReason;
                transactionToCancel.CanceledDate = DateTime.UtcNow;
                transactionToCancel.CanceledBy = request.CanceledBy;
                transactionToCancel.LastModifiedDate = DateTime.UtcNow;

                _context.StockTransactions.Update(transactionToCancel);

                // 2. Hoàn tác số lượng tồn kho thực tế trong ProductVariant
                // (Nếu nhập: trừ đi; Nếu xuất: trừ đi số âm thành cộng)
                productVariant.StockQuantity -= transactionToCancel.QuantityChange;
                _context.ProductVariants.Update(productVariant);

                var result = await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                if (result > 0)
                {
                    _logger.LogInformation("CancelStockTransaction success for id = {id}", id);
                    return Ok(CreateStockTransactionViewModel(transactionToCancel));
                }

                return BadRequest("Lỗi khi lưu dữ liệu hủy giao dịch.");
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError(ex, "CancelStockTransaction failed for id = {id}", id);
                return StatusCode(500, "Lỗi hệ thống khi hủy giao dịch kho.");
            }
        }

        /// <summary>
        /// Get stock transaction by filter and paging
        /// </summary>
        [HttpGet("filter")]
        public async Task<IActionResult> GetStockTransactions
        (
            [FromQuery] string? productName,
            [FromQuery] string? warehouseEmail,
            [FromQuery] bool? isCanceled, // THÊM PARAM LỌC THEO TRẠNG THÁI HỦY
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            _logger.LogInformation("Begin GetStockTransactions API");

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.StockTransactions.AsQueryable();

            if (isCanceled.HasValue)
            {
                query = query.Where(st => st.IsCanceled == isCanceled.Value);
            }

            if (!string.IsNullOrEmpty(productName))
            {
                var productIds = await _context.Products
                    .Where(p => p.Name.Contains(productName))
                    .Select(p => p.Id)
                    .ToListAsync();

                query = query.Where(st => productIds.Contains(st.ProductId));
            }

            if (!string.IsNullOrEmpty(warehouseEmail))
            {
                var warehouseIds = await _context.Warehouses
                    .Where(w => w.Email.Contains(warehouseEmail))
                    .Select(w => w.Id)
                    .ToListAsync();

                query = query.Where(st => warehouseIds.Contains(st.WarehouseId));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => CreateStockTransactionViewModel(x))
                .ToListAsync();

            var result = new Pagination<StockTransactionViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            };

            return Ok(result);
        }

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetStockTransactionsByWarehouseId(int warehouseId)
        {
            var stockTransactions = await _context.StockTransactions
                .Where(x => x.WarehouseId == warehouseId)
                .Select(x => CreateStockTransactionViewModel(x))
                .ToListAsync();

            return Ok(stockTransactions);
        }

        [HttpGet("reference/{referenceType}/{referenceId}")]
        public async Task<IActionResult> GetStockTransactionsByReference(Data.Enums.ReferenceType referenceType, int referenceId)
        {
            var stockTransactions = await _context.StockTransactions
                .Where(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId)
                .Select(x => CreateStockTransactionViewModel(x))
                .ToListAsync();

            return Ok(stockTransactions);
        }

        [HttpPost("importData")]
        public async Task<IActionResult> ImportData([FromBody] List<StockTransactionCreateRequest>? requests)
        {
            if (requests == null || requests.Count == 0) return BadRequest("No stock transaction requests provided.");

            var now = DateTime.UtcNow;

            var stockTransactions = requests.Select(r => new StockTransaction
            {
                ProductId = r.ProductId,
                WarehouseId = r.WarehouseId,
                QuantityChange = r.QuantityChange,
                TransactionType = (Data.Enums.StockTransactionType)r.TransactionType,
                ReferenceType = (Data.Enums.ReferenceType)r.ReferenceType,
                ProductVariantId = r.ProductVariantId,
                Note = r.Note,
                ReferenceId = r.ReferenceId,
                BalanceAfter = r.BalanceAfter,
                CreateDate = now,
                LastModifiedDate = now,
                IsCanceled = false
            }).ToList();

            try
            {
                _context.StockTransactions.AddRange(stockTransactions);
                var saved = await _context.SaveChangesAsync();

                if (saved > 0)
                    return Ok(new { Count = stockTransactions.Count, Ids = stockTransactions.Select(s => s.Id).ToList() });

                return BadRequest("No changes were saved to the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportData unexpected error");
                return StatusCode(500, "An unexpected error occurred while importing stock transactions.");
            }
        }

        [HttpPost("exportData")]
        public async Task<IActionResult> ExportData([FromBody] List<int>? ids, [FromQuery] bool idsAreProductIds = false)
        {
            try
            {
                var query = _context.StockTransactions.AsQueryable();

                if (ids != null && ids.Count > 0)
                {
                    query = idsAreProductIds
                        ? query.Where(st => ids.Contains(st.ProductId))
                        : query.Where(st => ids.Contains(st.Id));
                }

                var transactions = await query.OrderByDescending(st => st.Id).ToListAsync();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Stock Transactions");
                    worksheet.ShowGridLines = true;

                    // Set headers
                    worksheet.Cell(1, 1).Value = "Transaction ID";
                    worksheet.Cell(1, 2).Value = "Product ID";
                    worksheet.Cell(1, 3).Value = "Product Name";
                    worksheet.Cell(1, 4).Value = "Product Code";
                    worksheet.Cell(1, 5).Value = "Warehouse ID";
                    worksheet.Cell(1, 6).Value = "Quantity Change";
                    worksheet.Cell(1, 7).Value = "Transaction Type";
                    worksheet.Cell(1, 8).Value = "Reference Type";
                    worksheet.Cell(1, 9).Value = "Reference ID";
                    worksheet.Cell(1, 10).Value = "Balance After";
                    worksheet.Cell(1, 11).Value = "Create Date";
                    worksheet.Cell(1, 12).Value = "Last Modified Date";
                    worksheet.Cell(1, 13).Value = "Is Canceled";
                    worksheet.Cell(1, 14).Value = "Cancel Reason";
                    worksheet.Cell(1, 15).Value = "Canceled Date";
                    worksheet.Cell(1, 16).Value = "Canceled By";

                    var headerRow = worksheet.Row(1);
                    headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Font.FontColor = XLColor.White;
                    headerRow.Style.Font.FontSize = 11;
                    headerRow.Height = 25;

                    for (int col = 1; col <= 16; col++)
                    {
                        var cell = worksheet.Cell(1, col);
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D3D3D3");
                    }

                    int row = 2;
                    if (transactions.Count > 0)
                    {
                        var productIds = transactions.Select(t => t.ProductId).Distinct().ToList();
                        var products = await _context.Products
                            .Where(p => productIds.Contains(p.Id))
                            .Select(p => new { p.Id, p.Name, p.Code })
                            .ToDictionaryAsync(p => p.Id);

                        foreach (var t in transactions)
                        {
                            products.TryGetValue(t.ProductId, out var prod);

                            worksheet.Cell(row, 1).Value = t.Id;
                            worksheet.Cell(row, 2).Value = t.ProductId;
                            worksheet.Cell(row, 3).Value = prod?.Name ?? string.Empty;
                            worksheet.Cell(row, 4).Value = prod?.Code ?? string.Empty;
                            worksheet.Cell(row, 5).Value = t.WarehouseId;
                            worksheet.Cell(row, 6).Value = t.QuantityChange;
                            worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0;[Red]-#,##0";
                            worksheet.Cell(row, 7).Value = t.TransactionType.ToString();
                            worksheet.Cell(row, 8).Value = t.ReferenceType.ToString();
                            worksheet.Cell(row, 9).Value = t.ReferenceId;
                            worksheet.Cell(row, 10).Value = t.BalanceAfter;
                            worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0";

                            worksheet.Cell(row, 11).Value = t.CreateDate;
                            worksheet.Cell(row, 11).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";

                            if (t.LastModifiedDate.HasValue)
                            {
                                worksheet.Cell(row, 12).Value = t.LastModifiedDate.Value;
                                worksheet.Cell(row, 12).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
                            }
                            else
                            {
                                worksheet.Cell(row, 12).Value = string.Empty;
                            }

                            worksheet.Cell(row, 13).Value = t.IsCanceled ? "Yes" : "No";
                            worksheet.Cell(row, 14).Value = t.CancelReason ?? string.Empty;

                            if (t.CanceledDate.HasValue)
                            {
                                worksheet.Cell(row, 15).Value = t.CanceledDate.Value;
                                worksheet.Cell(row, 15).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
                            }
                            else
                            {
                                worksheet.Cell(row, 15).Value = string.Empty;
                            }

                            worksheet.Cell(row, 16).Value = t.CanceledBy ?? string.Empty;

                            var bg = (row % 2 == 0) ? XLColor.FromHtml("#F8F9FA") : XLColor.White;
                            for (int col = 1; col <= 16; col++)
                            {
                                var cell = worksheet.Cell(row, col);
                                cell.Style.Fill.BackgroundColor = bg;
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#E2E8F0");
                                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                                if (col == 3 || col == 4 || col == 14 || col == 16)
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                }
                                else if (col == 6 || col == 10)
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                }
                                else
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }
                            }

                            row++;
                        }
                    }
                    else if (idsAreProductIds && ids != null && ids.Count > 0)
                    {
                        var products = await _context.Products
                            .Where(p => ids.Contains(p.Id))
                            .OrderBy(p => p.Id)
                            .Select(p => new { p.Id, p.Name, p.Code })
                            .ToListAsync();

                        foreach (var p in products)
                        {
                            worksheet.Cell(row, 1).Value = string.Empty;
                            worksheet.Cell(row, 2).Value = p.Id;
                            worksheet.Cell(row, 3).Value = p.Name;
                            worksheet.Cell(row, 4).Value = p.Code;
                            worksheet.Cell(row, 5).Value = string.Empty;
                            worksheet.Cell(row, 6).Value = 0;
                            worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0;[Red]-#,##0";
                            worksheet.Cell(row, 7).Value = string.Empty;
                            worksheet.Cell(row, 8).Value = string.Empty;
                            worksheet.Cell(row, 9).Value = string.Empty;
                            worksheet.Cell(row, 10).Value = 0;
                            worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0";
                            worksheet.Cell(row, 11).Value = string.Empty;
                            worksheet.Cell(row, 12).Value = string.Empty;
                            worksheet.Cell(row, 13).Value = "No";
                            worksheet.Cell(row, 14).Value = string.Empty;
                            worksheet.Cell(row, 15).Value = string.Empty;
                            worksheet.Cell(row, 16).Value = string.Empty;

                            var bg = (row % 2 == 0) ? XLColor.FromHtml("#F8F9FA") : XLColor.White;
                            for (int col = 1; col <= 16; col++)
                            {
                                var cell = worksheet.Cell(row, col);
                                cell.Style.Fill.BackgroundColor = bg;
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#E2E8F0");
                                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                                if (col == 3 || col == 4 || col == 14 || col == 16)
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                }
                                else if (col == 6 || col == 10)
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                }
                                else
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }
                            }

                            row++;
                        }
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileBytes = stream.ToArray();
                        var fileName = $"stock_transactions_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
                        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExportData unexpected error");
                return StatusCode(500, "An unexpected error occurred while exporting stock transactions.");
            }
        }

        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetStockTransactionsByProductId(int productId)
        {
            var stockTransactions = await _context.StockTransactions
                .Where(x => x.ProductId == productId)
                .Select(x => CreateStockTransactionViewModel(x))
                .ToListAsync();

            return Ok(stockTransactions);
        }

        [HttpGet("showInfo/{id}")]
        public async Task<IActionResult> ShowInfo(int id)
        {
            var data = await _context.StockTransactions
                .Where(st => st.Id == id)
                .Select(st => new
                {
                    Product = _context.Products.Where(p => p.Id == st.ProductId).Select(p => new { p.Id, p.Name, p.Code, p.Description }).FirstOrDefault(),
                    ProductVariant = _context.ProductVariants.Where(pv => pv.Id == st.ProductVariantId).Select(pv => new { pv.Id, pv.Name, pv.SKU, pv.SellingPrice, pv.StockQuantity }).FirstOrDefault(),
                    Warehouse = _context.Warehouses.Where(w => w.Id == st.WarehouseId).Select(w => new { w.Id, w.Email }).FirstOrDefault(),
                    StockTransaction = new
                    {
                        st.Id,
                        st.QuantityChange,
                        st.TransactionType,
                        st.ReferenceType,
                        st.ReferenceId,
                        st.BalanceAfter,
                        st.CreateDate,
                        st.LastModifiedDate,
                        st.IsCanceled,       // THÊM: hiển thị trường hủy
                        st.CancelReason,
                        st.CanceledDate,
                        st.CanceledBy
                    }
                })
                .FirstOrDefaultAsync();

            return Ok(data);
        }

        private static StockTransactionViewModel CreateStockTransactionViewModel(StockTransaction x)
        {
            return new StockTransactionViewModel
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductVariantId = x.ProductVariantId,
                WarehouseId = x.WarehouseId,
                QuantityChange = x.QuantityChange,
                TransactionType = (ViewModels.Enums.StockTransactionType)x.TransactionType,
                ReferenceType = (ViewModels.Enums.ReferenceType)x.ReferenceType,
                ReferenceId = x.ReferenceId,
                BalanceAfter = x.BalanceAfter,
                CreateDate = x.CreateDate,
                LastModifiedDate = x.LastModifiedDate,
                // THÊM 4 TRƯỜNG DƯỚI ĐÂY (nhớ thêm properties vào class StockTransactionViewModel nhé)
                IsCanceled = x.IsCanceled,
                CancelReason = x.CancelReason,
                CanceledDate = x.CanceledDate,
                CanceledBy = x.CanceledBy
            };
        }

        [HttpPost("bulk-import-stock")]
        public async Task<IActionResult> BulkImportStock([FromBody] List<StockTransactionCreateRequest> requests)
        {
            if (requests == null || !requests.Any()) return BadRequest("Danh sách nhập kho không được để trống.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var results = new List<StockTransaction>();

                foreach (var request in requests)
                {
                    if (request.QuantityChange <= 0)
                        return BadRequest($"Sản phẩm (ID: {request.ProductId}) có QuantityChange phải lớn hơn 0.");

                    var productVariant = await _context.ProductVariants.FindAsync(request.ProductVariantId);
                    if (productVariant == null)
                        return BadRequest($"Không tìm thấy biến thể sản phẩm ID: {request.ProductVariantId}");

                    var lastBalance = await _context.StockTransactions
                        .Where(x => x.ProductId == request.ProductId && x.WarehouseId == request.WarehouseId)
                        .OrderByDescending(x => x.Id)
                        .Select(x => x.BalanceAfter)
                        .FirstOrDefaultAsync();

                    var newBalance = lastBalance + request.QuantityChange;

                    var stockTransaction = new StockTransaction
                    {
                        ProductId = request.ProductId,
                        ProductVariantId = request.ProductVariantId,
                        WarehouseId = request.WarehouseId,
                        QuantityChange = request.QuantityChange,
                        TransactionType = (Data.Enums.StockTransactionType)request.TransactionType,
                        ReferenceType = (Data.Enums.ReferenceType)request.ReferenceType,
                        ReferenceId = request.ReferenceId,
                        Note = request.Note,
                        CreateDate = now,
                        LastModifiedDate = now,
                        BalanceAfter = newBalance,
                        IsCanceled = false
                    };

                    productVariant.StockQuantity += request.QuantityChange;

                    _context.StockTransactions.Add(stockTransaction);
                    _context.ProductVariants.Update(productVariant);

                    results.Add(stockTransaction);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(results.Select(x => CreateStockTransactionViewModel(x)));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "BulkImportStock failed");
                return StatusCode(500, "Lỗi hệ thống khi nhập kho hàng loạt.");
            }
        }

        [HttpPost("bulk-export-stock")]
        public async Task<IActionResult> BulkExportStock([FromBody] List<StockTransactionCreateRequest> requests)
        {
            if (requests == null || !requests.Any()) return BadRequest("Danh sách xuất kho không được để trống.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;
                var results = new List<StockTransaction>();

                foreach (var request in requests)
                {
                    if (request.QuantityChange >= 0)
                        return BadRequest($"Sản phẩm (ID: {request.ProductId}) có QuantityChange phải nhỏ hơn 0.");

                    var productVariant = await _context.ProductVariants.FindAsync(request.ProductVariantId);
                    if (productVariant == null)
                        return BadRequest($"Không tìm thấy biến thể sản phẩm ID: {request.ProductVariantId}");

                    if (productVariant.StockQuantity + request.QuantityChange < 0)
                        return BadRequest($"Sản phẩm (ID: {request.ProductId}) không đủ tồn kho để xuất. Hiện có: {productVariant.StockQuantity}");

                    var lastBalance = await _context.StockTransactions
                        .Where(x => x.ProductId == request.ProductId && x.WarehouseId == request.WarehouseId)
                        .OrderByDescending(x => x.Id)
                        .Select(x => x.BalanceAfter)
                        .FirstOrDefaultAsync();

                    var newBalance = lastBalance + request.QuantityChange;

                    var stockTransaction = new StockTransaction
                    {
                        ProductId = request.ProductId,
                        ProductVariantId = request.ProductVariantId,
                        WarehouseId = request.WarehouseId,
                        QuantityChange = request.QuantityChange,
                        TransactionType = (Data.Enums.StockTransactionType)request.TransactionType,
                        ReferenceType = (Data.Enums.ReferenceType)request.ReferenceType,
                        ReferenceId = request.ReferenceId,
                        Note = request.Note,
                        CreateDate = now,
                        LastModifiedDate = now,
                        BalanceAfter = newBalance,
                        IsCanceled = false
                    };

                    productVariant.StockQuantity += request.QuantityChange;

                    _context.StockTransactions.Add(stockTransaction);
                    _context.ProductVariants.Update(productVariant);

                    results.Add(stockTransaction);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(results.Select(x => CreateStockTransactionViewModel(x)));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "BulkExportStock failed");
                return StatusCode(500, "Lỗi hệ thống khi xuất kho hàng loạt.");
            }
        }

        /// <summary>
        /// Xử lý một Phiếu Nhập / Xuất kho bằng cách phân loại dựa trên StockTransactionType
        /// </summary>
        [HttpPost("process-document")]
        public async Task<IActionResult> ProcessStockDocument([FromBody] StockDocumentRequest request)
        {
            _logger.LogInformation("Begin ProcessStockDocument API - TransactionType: {Type}", request.TransactionType);

            if (request.Items == null || !request.Items.Any())
                return BadRequest("Danh sách sản phẩm (items) không được để trống.");

            bool isImport = true;
            switch (request.TransactionType)
            {
                case ViewModels.Enums.StockTransactionType.PurchaseReceipt:
                case ViewModels.Enums.StockTransactionType.CustomerReturn:
                case ViewModels.Enums.StockTransactionType.TransferIn:
                case ViewModels.Enums.StockTransactionType.AdjustmentIncrease:
                    isImport = true; // Nhập kho -> Cộng
                    break;

                case ViewModels.Enums.StockTransactionType.SalesIssue:
                case ViewModels.Enums.StockTransactionType.SupplierReturn:
                case ViewModels.Enums.StockTransactionType.TransferOut:
                case ViewModels.Enums.StockTransactionType.AdjustmentDecrease:
                case ViewModels.Enums.StockTransactionType.Damaged:
                    isImport = false; // Xuất kho -> Trừ
                    break;

                default:
                    return BadRequest("Loại giao dịch (TransactionType) không hợp lệ để thực hiện phiếu này.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.UtcNow;

                foreach (var item in request.Items)
                {
                    if (item.Quantity <= 0)
                        return BadRequest($"Sản phẩm (ID: {item.ProductId}) phải có số lượng lớn hơn 0.");

                    var productVariant = await _context.ProductVariants.FindAsync(item.ProductVariantId);
                    if (productVariant == null)
                        return BadRequest($"Không tìm thấy biến thể sản phẩm ID: {item.ProductVariantId}");

                    int actualQuantityChange = isImport ? item.Quantity : -item.Quantity;

                    if (!isImport && productVariant.StockQuantity + actualQuantityChange < 0)
                        return BadRequest($"Sản phẩm (ID: {item.ProductId}) không đủ tồn kho để xuất. Hiện có: {productVariant.StockQuantity}");

                    var lastBalance = await _context.StockTransactions
                        .Where(x => x.ProductId == item.ProductId && x.WarehouseId == request.WarehouseId)
                        .OrderByDescending(x => x.Id)
                        .Select(x => x.BalanceAfter)
                        .FirstOrDefaultAsync();

                    var newBalance = lastBalance + actualQuantityChange;

                    var stockTransaction = new StockTransaction
                    {
                        ProductId = item.ProductId,
                        ProductVariantId = item.ProductVariantId,
                        WarehouseId = request.WarehouseId,
                        QuantityChange = actualQuantityChange,

                        TransactionType = (WarehouseManagement.BackendServer.Data.Enums.StockTransactionType)(int)request.TransactionType,

                        ReferenceType = Data.Enums.ReferenceType.Other,
                        ReferenceId = 0,
                        Note = $"{request.ReferenceCode} - {request.Note}",
                        CreateDate = now,
                        LastModifiedDate = now,
                        BalanceAfter = newBalance,
                        IsCanceled = false
                    };

                    productVariant.StockQuantity += actualQuantityChange;

                    _context.StockTransactions.Add(stockTransaction);
                    _context.ProductVariants.Update(productVariant);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new
                {
                    supplierId = request.SupplierId,
                    supplierName = request.SupplierName,
                    receiptDate = request.ReceiptDate,
                    referenceCode = request.ReferenceCode,
                    note = request.Note,
                    totalAmount = request.TotalAmount,
                    createDate = now,
                    lastModifiedDate = now,
                    isCanceled = false,
                    cancelReason = (string?)null,
                    canceledDate = (DateTime?)null,
                    canceledBy = (string?)null,
                    items = request.Items.Select(i => new
                    {
                        productId = i.ProductId,
                        productVariantId = i.ProductVariantId,
                        quantity = i.Quantity, // Trả lại số nguyên dương như lúc gọi API
                        unitPrice = i.UnitPrice,
                        totalPrice = i.TotalPrice
                    }).ToList()
                };

                _logger.LogInformation("ProcessStockDocument success.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProcessStockDocument failed");
                return StatusCode(500, "Lỗi hệ thống khi xử lý chứng từ kho.");
            }
        }
    }
}
