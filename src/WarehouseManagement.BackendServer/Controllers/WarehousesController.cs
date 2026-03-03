using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController(ApplicationDbContext _context, ILogger<WarehousesController> _logger) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllWarehouses()
        {
            var listWarehouses = await _context.Warehouses
                .Where(w => !w.IsDeleted)
                .ToListAsync();
            
            return Ok(listWarehouses);
        }
    }
}
