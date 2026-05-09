using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Services.Interfaces;
using WarehouseManagement.ViewModels.Contents;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockManagementController : ControllerBase
    {
        private readonly IStockTransactionService _stockService;
        private readonly ILogger<StockManagementController> _logger;

        public StockManagementController(IStockTransactionService stockService, ILogger<StockManagementController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(StockTransferRequest request)
        {
            _logger.LogInformation("Begin Transfer API");
            var ok = await _stockService.TransferAsync(request);
            if (!ok) return BadRequest("Transfer failed (insufficient stock or invalid data).");
            return Ok();
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock([FromQuery] int threshold = 10)
        {
            var items = await _stockService.GetLowStockAsync(threshold);
            return Ok(items);
        }

        [HttpGet("variant-stock/{variantId}")]
        public async Task<IActionResult> GetVariantStock(int variantId, [FromQuery] int? warehouseId = null)
        {
            var qty = await _stockService.GetVariantStockAsync(variantId, warehouseId);
            return Ok(new { VariantId = variantId, WarehouseId = warehouseId, StockQuantity = qty });
        }
    }
}