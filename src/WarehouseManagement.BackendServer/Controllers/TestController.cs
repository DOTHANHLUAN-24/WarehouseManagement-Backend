using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.ViewModels.Contents.Products;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(ApplicationDbContext _context) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API Connected Successfully");
        }

        [HttpGet("allProduct")]
        public async Task<IActionResult> GetAllProducts()
        {
            var productList = await (
                from p in _context.Products
                join pi in _context.ProductImages
                    on p.Id equals pi.ProductId
                where
                    !p.IsDeleted
                   && !pi.IsDeleted
                   && pi.IsDefault
                select new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    Code = p.Code,
                    IsActive = p.IsActive,
                    ImageUrl = pi.ImageUrl,
                    IsDefault = pi.IsDefault
                }
            ).ToListAsync();

            return Ok(productList);
        }
    }
}
