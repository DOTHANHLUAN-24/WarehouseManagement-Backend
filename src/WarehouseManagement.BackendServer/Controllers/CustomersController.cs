using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Data;

namespace WarehouseManagement.BackendServer.Controllers
{
    public class CustomersController(ApplicationDbContext _context, ILogger<CustomersController> _logger)
    {
    }
}
