using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.ViewModels.Contents.StockTransactions;
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
        [HttpGet]
        public async Task<IActionResult> GetAllStockTransactions()
        {
            var listStockTransactions = await _context.StockTransactions
                .Select(x => new StockTransactionViewModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    WarehouseId = x.WarehouseId,
                    QuantityChange = x.QuantityChange,
                    TransactionType = (ViewModels.Enums.StockTransactionType)x.TransactionType,
                    ReferenceType = (ViewModels.Enums.ReferenceType)x.ReferenceType,
                    ReferenceId = x.ReferenceId,
                    BalanceAfter = x.BalanceAfter,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate
                }).ToListAsync();

            return Ok(listStockTransactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockTransactionById(int id)
        {
            var stockTransaction = await _context.StockTransactions
                .Where(x => x.Id == id)
                .Select(x => new StockTransactionViewModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    WarehouseId = x.WarehouseId,
                    QuantityChange = x.QuantityChange,
                    TransactionType = (ViewModels.Enums.StockTransactionType)x.TransactionType,
                    ReferenceType = (ViewModels.Enums.ReferenceType)x.ReferenceType,
                    ReferenceId = x.ReferenceId,
                    BalanceAfter = x.BalanceAfter,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate
                }).FirstOrDefaultAsync();
            if (stockTransaction == null)
                return NotFound();

            return Ok(stockTransaction);
        }

        [HttpPost]
        public async Task<IActionResult> PostStockTransaction(StockTransactionCreateRequest request)
        {
            var stockTransaction = new StockTransaction
            {
                ProductId = request.ProductId,
                WarehouseId = request.WarehouseId,
                QuantityChange = request.QuantityChange,
                TransactionType = (StockTransactionType)request.TransactionType,
                ReferenceType = (ReferenceType)request.ReferenceType,
                ReferenceId = request.ReferenceId,
                BalanceAfter = request.BalanceAfter, // Cần tính toán lại số lượng tồn sau khi thực hiện giao dịch
                CreateDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            _context.StockTransactions.Add(stockTransaction);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return CreatedAtAction(nameof(GetStockTransactionById), new { id = stockTransaction.Id }, stockTransaction);
            return BadRequest();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetStockTransactions
        (
            [FromQuery] string? productName,
            [FromQuery] string? warehouseEmail,
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.StockTransactions.AsQueryable();

            // Filter by product name
            if (!string.IsNullOrEmpty(productName))
            {
                var productIds = await _context.Products
                    .Where(p => p.Name.Contains(productName))
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                query = query.Where(st => st.ProductId == productIds);
            }

            // Filter by warehouse name
            if (!string.IsNullOrEmpty(warehouseEmail))
            {
                var warehouseIds = await _context.Warehouses
                    .Where(w => w.Email.Contains(warehouseEmail))
                    .Select(w => w.Id)
                    .FirstOrDefaultAsync();

                query = query.Where(st => st.WarehouseId == warehouseIds);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StockTransactionViewModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    WarehouseId = x.WarehouseId,
                    QuantityChange = x.QuantityChange,
                    TransactionType = (ViewModels.Enums.StockTransactionType)x.TransactionType,
                    ReferenceType = (ViewModels.Enums.ReferenceType)x.ReferenceType,
                    ReferenceId = x.ReferenceId,
                    BalanceAfter = x.BalanceAfter,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate
                }).ToListAsync();

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
                .Select(x => new StockTransactionViewModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    WarehouseId = x.WarehouseId,
                    QuantityChange = x.QuantityChange,
                    TransactionType = (ViewModels.Enums.StockTransactionType)x.TransactionType,
                    ReferenceType = (ViewModels.Enums.ReferenceType)x.ReferenceType,
                    ReferenceId = x.ReferenceId,
                    BalanceAfter = x.BalanceAfter,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate
                }).ToListAsync();

            return Ok(stockTransactions);
        }

        [HttpGet("stockTransactions/reference/{referenceType}/{referenceId}")]
        public async Task<IActionResult> GetStockTransactionsByReference(ReferenceType referenceType, int referenceId)
        {
            var stockTransactions = await _context.StockTransactions
                .Where(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId)
                .Select(x => new StockTransactionViewModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    WarehouseId = x.WarehouseId,
                    QuantityChange = x.QuantityChange,
                    TransactionType = (ViewModels.Enums.StockTransactionType)x.TransactionType,
                    ReferenceType = (ViewModels.Enums.ReferenceType)x.ReferenceType,
                    ReferenceId = x.ReferenceId,
                    BalanceAfter = x.BalanceAfter,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate
                }).ToListAsync();

            return Ok(stockTransactions);
        }
    }
}
