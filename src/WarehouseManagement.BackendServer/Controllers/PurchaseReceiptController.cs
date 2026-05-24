using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Services;
using WarehouseManagement.ViewModels.Contents.PurchaseReceipts;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseReceiptController : ControllerBase
    {
        private readonly IPurchaseReceiptService _service;
        private readonly ILogger<PurchaseReceiptController> _logger;

        public PurchaseReceiptController(IPurchaseReceiptService service, ILogger<PurchaseReceiptController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _service.GetByIdAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseReceiptRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.PurchaseId }, created);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating purchase receipt");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseReceiptRequest request)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, request);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
