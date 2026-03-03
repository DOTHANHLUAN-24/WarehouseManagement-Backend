using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Warehouses;
using WarehouseManagement.ViewModels.Systems;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController
        (
            ApplicationDbContext _context,
            ILogger<WarehousesController> _logger
        ) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllWarehouses()
        {
            var listWarehouses = await _context.Warehouses
                .Where(w => !w.IsDeleted)
                .Select(w => new WarehouseViewModel
                {
                    Id = w.Id,
                    Location = w.Location,
                    Capacity = w.Capacity,
                    Email = w.Email,
                    IsDeleted = w.IsDeleted
                })
                .ToListAsync();

            return Ok(listWarehouses);
        }

        [HttpPost]
        public async Task<IActionResult> PostWarehouse(WarehouseBase warehouseBase)
        {
            var warehouse = new Warehouse
            {
                Location = warehouseBase.Location,
                Capacity = warehouseBase.Capacity,
                Email = warehouseBase.Email,
                IsDeleted = warehouseBase.IsDeleted
            };

            _context.Warehouses.Add(warehouse);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse);

            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
                return NotFound();

            var warehouseViewModel = new WarehouseViewModel
            {
                Id = warehouse.Id,
                Location = warehouse.Location,
                Capacity = warehouse.Capacity,
                Email = warehouse.Email,
                IsDeleted = warehouse.IsDeleted
            };

            return Ok(warehouseViewModel);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetWarehouses(
            [FromQuery] string? location,
            [FromQuery] int? capacity,
            [FromQuery] string? email,
            [FromQuery] string? filter,
            int pageIndex = 1,
            int pageSize = 10)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Warehouses
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // Filter by location
            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query
                    .Where(x => x.Location.Contains(location));
            }

            // Filter by capacity
            if (capacity != null)
            {
                query = query
                    .Where(x => x.Capacity == capacity);
            }

            // Filter by email
            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query
                    .Where(x => x.Email.Contains(email));
            }

            // Global filter
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query
                    .Where(x => x.Location.Contains(filter) ||
                    x.Email.Contains(filter));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new WarehouseViewModel
                {
                    Id = x.Id,
                    Capacity = x.Capacity,
                    Location = x.Location,
                    Email = x.Email,
                    IsDeleted = x.IsDeleted,
                })
                .ToListAsync();

            var result = new Pagination<WarehouseViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouse(int id, [FromBody] WarehouseUpdateRequest request)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
                return NotFound();

            warehouse.Email = request.Email;
            warehouse.Location = request.Location;
            warehouse.Capacity = request.Capacity;

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return NoContent();
            else
                return BadRequest();
        }

        [HttpGet("trash")]
        public async Task<IActionResult> GetWarehousesInTrash()
        {
            var warehousesInTrash = await _context.Warehouses
                .Where(x => x.IsDeleted)
                .Select(x => new WarehouseViewModel
                {
                    Id = x.Id,
                    Capacity = x.Capacity,
                    Email = x.Email,
                    Location = x.Location,
                    IsDeleted = x.IsDeleted,
                })
                .ToListAsync();

            return Ok(warehousesInTrash);
        }

        [HttpDelete("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteWarehouse(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
                return NotFound();

            if (warehouse.IsDeleted)
                return BadRequest();

            warehouse.IsDeleted = true;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreWarehouse(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
                return NotFound();

            if (!warehouse.IsDeleted)
                return BadRequest();

            warehouse.IsDeleted = false;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }

        [HttpDelete("{id}/permanent-delete")]
        public async Task<IActionResult> PermanentDeleteWarehouse(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
                return NotFound();

            if (!warehouse.IsDeleted)
                return BadRequest();

            _context.Warehouses.Remove(warehouse);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();

            return BadRequest();
        }
    }
}
