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
        /// <summary>
        /// Get all stock transactions in the system
        /// </summary>
        /// <returns>List stock transactions</returns>
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
        /// <param name="id">Stock transaction id</param>
        /// <returns>The stock transaction or not found</returns>
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
        /// <param name="request">Stock transaction model</param>
        /// <returns>Result of create process</returns>
        [HttpPost]
        public async Task<IActionResult> PostStockTransaction(StockTransactionCreateRequest request)
        {
            _logger.LogInformation("Begin PostStockTransaction API");

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
            {
                _logger.LogInformation("PostStockTransaction success with id = {id}", stockTransaction.Id);

                return CreatedAtAction(nameof(GetStockTransactionById), new { id = stockTransaction.Id }, stockTransaction);
            }

            _logger.LogWarning("PostStockTransaction failed to save changes");

            return BadRequest();
        }

        /// <summary>
        /// Get stock transaction by filter and paging
        /// </summary>
        /// <param name="productName">Product name</param>
        /// <param name="warehouseEmail">Warehouse email</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List stock transactions by filter</returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetStockTransactions
        (
            [FromQuery] string? productName,
            [FromQuery] string? warehouseEmail,
            int pageIndex = 1,
            int pageSize = 10
        )
        {
            _logger.LogInformation("Begin GetStockTransactions API with " +
                "product name = {productName}, warehouseEmail = {warehouseEmail}, pageIndex = {pageIndex}, pageSize = {pageSize}", 
                productName, warehouseEmail, pageIndex, pageSize);

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.StockTransactions.AsQueryable();

            // Filter by product name
            if (!string.IsNullOrEmpty(productName))
            {
                _logger.LogInformation("Begin get stock transactions by the product name = {productName}", productName);

                var productIds = await _context.Products
                    .Where(p => p.Name.Contains(productName))
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                query = query.Where(st => st.ProductId == productIds);
            }

            // Filter by warehouse name
            if (!string.IsNullOrEmpty(warehouseEmail))
            {
                _logger.LogInformation("Begin get stock transactions by the warehouse email = {warehouseEmail}", warehouseEmail);

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
                .Select(x => CreateStockTransactionViewModel(x))
                .ToListAsync();

            var result = new Pagination<StockTransactionViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            };

            _logger.LogInformation("GetStockTransactions success. Total records = {TotalRecords}, items = {items}", totalRecords, items);

            return Ok(result);
        }

        /// <summary>
        /// Get stock transactions by warehouse id
        /// </summary>
        /// <param name="warehouseId">Warehouse id</param>
        /// <returns>The stock transaction or not found</returns>
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetStockTransactionsByWarehouseId(int warehouseId)
        {
            _logger.LogInformation("Begin GetStockTransactionsByWarehouseId API");

            var stockTransactions = await _context.StockTransactions
                .Where(x => x.WarehouseId == warehouseId)
                .Select(x => CreateStockTransactionViewModel(x))
                .ToListAsync();

            _logger.LogInformation("GetStockTransactionsByWarehouseId success");

            return Ok(stockTransactions);
        }

        /// <summary>
        /// Get stock transaction by reference id and reference type
        /// </summary>
        /// <param name="referenceType">Reference type</param>
        /// <param name="referenceId">Reference id</param>
        /// <returns>The stock transactions by reference</returns>
        [HttpGet("stockTransactions/reference/{referenceType}/{referenceId}")]
        public async Task<IActionResult> GetStockTransactionsByReference(ReferenceType referenceType, int referenceId)
        {
            _logger.LogInformation("Begin GetStockTransactionsByReference API");

            var stockTransactions = await _context.StockTransactions
                .Where(x => x.ReferenceType == referenceType && x.ReferenceId == referenceId)
                .Select(x => CreateStockTransactionViewModel(x))
                .ToListAsync();

            _logger.LogInformation("GetStockTransactionsByReference success");

            return Ok(stockTransactions);
        }

        /// <summary>
        /// Create stock transaction view model
        /// </summary>
        /// <param name="x">Stock transaction request</param>
        /// <returns>Stock transaction view model</returns>
        private static StockTransactionViewModel CreateStockTransactionViewModel(StockTransaction x)
        {
            return new StockTransactionViewModel
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
            };
        }
    }
}
