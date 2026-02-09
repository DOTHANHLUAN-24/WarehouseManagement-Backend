using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Data;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ApplicationDbContext _context, ILogger<CategoriesController> _logger) : BaseController
    {

        [HttpPost]
        public Task<IActionResult> PostCategory([FromBody] Categ)
        {

        }


    }
}
