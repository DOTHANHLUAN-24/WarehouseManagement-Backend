using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Data;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionsController(ApplicationDbContext _context) : BaseController
    {
        
    }
}
