using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Contents.Suppliers;
using WarehouseManagement.ViewModels.Systems;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController
        (
            ApplicationDbContext _context, 
            ILogger<SuppliersController> _logger
        ) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> PostSupplier([FromBody] SupplierCreateRequest request)
        {
            var supplier = new Supplier
            {
                SupplierName = request.SupplierName,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                Address = request.Address,
                Email = request.Email,
                IsActive = request.IsActive,
                IsDeleted = request.IsDeleted
            };

            _context.Suppliers.Add(supplier);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return CreatedAtAction(nameof(GetById), supplier);
            else
                return BadRequest("Failed to create supplier");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            var supplierViewModel = new SupplierViewModel
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Address = supplier.Address,
                Email = supplier.Email,
                IsActive = supplier.IsActive,
                IsDeleted = supplier.IsDeleted
            };

            return Ok(supplierViewModel);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _context.Suppliers
                .Where(s => !s.IsDeleted)
                .Select(s => new SupplierViewModel
                {
                    Id = s.Id,
                    SupplierName = s.SupplierName,
                    ContactPerson = s.ContactPerson,
                    Phone = s.Phone,
                    Address = s.Address,
                    Email = s.Email,
                    IsActive = s.IsActive,
                    IsDeleted = s.IsDeleted
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers(
            [FromQuery] string? supplierName,
            [FromQuery] string? contactPerson,
            [FromQuery] bool? isActive,
            [FromQuery] string? filter,
            int pageIndex = 1,
            int pageSize = 10)
        {
            // normalize paging
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Suppliers
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // Filter by supplierName
            if (!string.IsNullOrWhiteSpace(supplierName))
            {
                query = query.Where(x =>
                    x.SupplierName.Contains(supplierName));
            }

            // Filter by contactPerson
            if (!string.IsNullOrWhiteSpace(contactPerson))
            {
                query = query.Where(x =>
                    x.ContactPerson != null &&
                    x.ContactPerson.Contains(contactPerson));
            }

            // Filter by status
            if (isActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == isActive.Value);
            }

            // Global filter
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(x =>
                    x.SupplierName.Contains(filter) ||
                    (x.ContactPerson != null &&
                     x.ContactPerson.Contains(filter)));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SupplierViewModel
                {
                    Id = x.Id,
                    SupplierName = x.SupplierName,
                    ContactPerson = x.ContactPerson,
                    Phone = x.Phone,
                    Address = x.Address,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync();

            var result = new Pagination<SupplierViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, [FromBody] SupplierUpdateRequest request)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            supplier.SupplierName = request.SupplierName;
            supplier.ContactPerson = request.ContactPerson;
            supplier.Phone = request.Phone;
            supplier.Address = request.Address;
            supplier.Email = request.Email;
            supplier.IsActive = request.IsActive;
            supplier.IsDeleted = request.IsDeleted;

            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return NoContent();
            else
                return BadRequest("Failed to update supplier");
        }

        [HttpGet("trash")]
        public async Task<IActionResult> GetSupplierInTrash()
        {
            var suppliersInTrash = await _context.Suppliers
                .Where(x => x.IsDeleted)
                .Select(x => new SupplierViewModel
                {
                    Id = x.Id,
                    SupplierName = x.SupplierName,
                    ContactPerson = x.ContactPerson,
                    Phone = x.Phone,
                    Address = x.Address,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync();

            return Ok(suppliersInTrash);
        }

        [HttpDelete("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            if (supplier.IsDeleted)
                return BadRequest("Supplier is already in trash");

            supplier.IsDeleted = true;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();
            else
                return BadRequest("Failed to soft delete supplier");
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            if (!supplier.IsDeleted)
                return BadRequest("Supplier is not in trash");

            supplier.IsDeleted = false;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();
            else
                return BadRequest("Failed to restore supplier");
        }


        [HttpDelete("{id}/permanent-delete")]
        public async Task<IActionResult> PermanentDeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            if (!supplier.IsDeleted)
                return BadRequest("Supplier must be in trash before permanent delete");

            _context.Suppliers.Remove(supplier);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return NoContent();
            else
                return BadRequest("Failed to permanently delete supplier");
        }
    }
}
