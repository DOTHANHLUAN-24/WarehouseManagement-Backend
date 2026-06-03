using Microsoft.AspNetCore.Mvc;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/v1/auth/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // Trả về 200 OK kèm dòng chữ thông báo
            return Ok(new { status = "Healthy", message = "Server is alive!" });
        }
    }
}