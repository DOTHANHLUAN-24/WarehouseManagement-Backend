using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Data;

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
        [HttpGet("all")]
        public Task<IActionResult> GetAllStockTransactions()
        {
            var listStockTransactions = _context.StockTransactions;
                
            return Task.FromResult<IActionResult>(Ok(listStockTransactions));
        }
    }
}
