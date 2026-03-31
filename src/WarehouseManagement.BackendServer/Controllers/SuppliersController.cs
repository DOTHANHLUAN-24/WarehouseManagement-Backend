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
        /// <summary>
        /// Creates a new supplier based on the provided request data. 
        /// The supplier is added to the database context and saved. 
        /// If the creation is successful, it returns a 201 Created response with the created supplier's details. If the creation fails, it returns a 400 Bad Request response.
        /// </summary>
        /// <param name="request">Supplier model</param>
        /// <returns>Result of create process</returns>
        [HttpPost]
        public async Task<IActionResult> PostSupplier([FromBody] SupplierCreateRequest request)
        {
            _logger.LogInformation("Creating new supplier with name: {SupplierName}", request.SupplierName);

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

            _logger.LogInformation("Adding supplier to database context");

            _context.Suppliers.Add(supplier);

            _logger.LogInformation("Saving changes to database");

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Supplier created successfully with ID: {SupplierId}", supplier.Id);

                return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
            }
            else
            {
                _logger.LogError("Failed to create supplier with name: {SupplierName}", request.SupplierName);

                return BadRequest("Failed to create supplier");
            }
        }

        /// <summary>
        /// Get supplier by id. If the supplier is found, it returns a 200 OK response with the supplier's details. 
        /// If the supplier is not found, it returns a 404 Not Found response.
        /// </summary>
        /// <param name="id">Supplier id</param>
        /// <returns>Result of get process</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Retrieving supplier with ID: {SupplierId}", id);

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID: {SupplierId} not found", id);

                return NotFound();
            }

            _logger.LogInformation("Supplier with ID: {SupplierId} retrieved successfully", id);

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

            _logger.LogInformation("Returning supplier view model for supplier with ID: {SupplierId}", id);

            return Ok(supplierViewModel);
        }

        /// <summary>
        /// Get list of suppliers that are not deleted. 
        /// It returns a 200 OK response with the list of suppliers.
        /// </summary>
        /// <returns>List of supplier</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllSuppliers()
        {
            _logger.LogInformation("Retrieving all suppliers that are not deleted");

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

            _logger.LogInformation("Retrieved {SupplierCount} suppliers", suppliers.Count);

            return Ok(suppliers);
        }

        /// <summary>
        /// Get list of suppliers with optional filters for supplier name, contact person, and active status.
        /// </summary>
        /// <param name="supplierName">Supplier name</param>
        /// <param name="contactPerson">Contact person</param>
        /// <param name="isActive">Is active</param>
        /// <param name="filter">Filter</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List supplier with paging</returns>
        [HttpGet]
        public async Task<IActionResult> GetSuppliers(
            [FromQuery] string? supplierName,
            [FromQuery] string? contactPerson,
            [FromQuery] bool? isActive,
            [FromQuery] string? filter,
            int pageIndex = 1,
            int pageSize = 10)
        {
            _logger.LogInformation("Retrieving suppliers with filters - SupplierName: {SupplierName}, ContactPerson: {ContactPerson}, IsActive: {IsActive}, Filter: {Filter}, PageIndex: {PageIndex}, PageSize: {PageSize}",
                supplierName, contactPerson, isActive, filter, pageIndex, pageSize);

            // normalize paging
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            pageSize = pageSize <= 0 ? 10 : pageSize;

            _logger.LogInformation("Normalized paging parameters - PageIndex: {PageIndex}, PageSize: {PageSize}", pageIndex, pageSize);

            var query = _context.Suppliers
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // Filter by supplierName
            if (!string.IsNullOrWhiteSpace(supplierName))
            {
                _logger.LogInformation("Applying filter for SupplierName containing: {SupplierName}", supplierName);

                query = query.Where(x =>
                    x.SupplierName.Contains(supplierName));
            }

            // Filter by contactPerson
            if (!string.IsNullOrWhiteSpace(contactPerson))
            {
                _logger.LogInformation("Applying filter for ContactPerson containing: {ContactPerson}", contactPerson);

                query = query.Where(x =>
                    x.ContactPerson != null &&
                    x.ContactPerson.Contains(contactPerson));
            }

            // Filter by status
            if (isActive.HasValue)
            {
                _logger.LogInformation("Applying filter for IsActive: {IsActive}", isActive.Value);

                query = query.Where(x =>
                    x.IsActive == isActive.Value);
            }

            // Global filter
            if (!string.IsNullOrWhiteSpace(filter))
            {
                _logger.LogInformation("Applying global filter for SupplierName or ContactPerson containing: {Filter}", filter);

                query = query.Where(x =>
                    x.SupplierName.Contains(filter) ||
                    (x.ContactPerson != null &&
                     x.ContactPerson.Contains(filter)));
            }

            _logger.LogInformation("Executing query to count total records");

            var totalRecords = await query.CountAsync();

            _logger.LogInformation("Total records found: {TotalRecords}", totalRecords);

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

            _logger.LogInformation("Retrieved {ItemCount} items for current page", items.Count);

            var result = new Pagination<SupplierViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            };

            _logger.LogInformation("Returning paginated result with {ItemCount} items and {TotalRecords} total records", items.Count, totalRecords);

            return Ok(result);
        }

        /// <summary>
        /// Update supplier by id. 
        /// If the supplier is found and updated successfully, it returns a 204 No Content response.
        /// </summary>
        /// <param name="id">Supplier id</param>
        /// <param name="request">Supplier model</param>
        /// <returns>Result of update process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, [FromBody] SupplierUpdateRequest request)
        {
            _logger.LogInformation("Updating supplier with ID: {SupplierId}", id);

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID: {SupplierId} not found for update", id);

                return NotFound();
            }

            _logger.LogInformation("Updating properties of supplier with ID: {SupplierId}", id);

            supplier.SupplierName = request.SupplierName;
            supplier.ContactPerson = request.ContactPerson;
            supplier.Phone = request.Phone;
            supplier.Address = request.Address;
            supplier.Email = request.Email;
            supplier.IsActive = request.IsActive;
            supplier.IsDeleted = request.IsDeleted;

            _logger.LogInformation("Saving changes to database for supplier with ID: {SupplierId}", id);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Supplier with ID: {SupplierId} updated successfully", id);

                return NoContent();
            }
            else
            {
                _logger.LogError("Failed to update supplier with ID: {SupplierId}", id);

                return BadRequest("Failed to update supplier");
            }
        }

        /// <summary>
        /// Get list of suppliers that are marked as deleted (in trash).
        /// </summary>
        /// <returns>List supplier in the trash</returns>
        [HttpGet("trash")]
        public async Task<IActionResult> GetSupplierInTrash()
        {
            _logger.LogInformation("Retrieving suppliers in trash");

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

            _logger.LogInformation("Retrieved {SupplierCount} suppliers in trash", suppliersInTrash.Count);

            return Ok(suppliersInTrash);
        }

        /// <summary>
        /// Soft delete supplier by id. 
        /// If the supplier is found and marked as deleted successfully, it returns a 204 No Content response.
        /// </summary>
        /// <param name="id">Supplier id</param>
        /// <returns>Result of soft process</returns>
        [HttpDelete("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteSupplier(int id)
        {
            _logger.LogInformation("Soft deleting supplier with ID: {SupplierId}", id);

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID: {SupplierId} not found for soft delete", id);

                return NotFound();
            }

            if (supplier.IsDeleted)
            {
                _logger.LogWarning("Supplier with ID: {SupplierId} is already in trash", id);

                return BadRequest("Supplier is already in trash");
            }

            _logger.LogInformation("Marking supplier with ID: {SupplierId} as deleted", id);

            supplier.IsDeleted = true;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Supplier with ID: {SupplierId} soft deleted successfully", id);

                return NoContent();
            }
            else
            {
                _logger.LogError("Failed to soft delete supplier with ID: {SupplierId}", id);

                return BadRequest("Failed to soft delete supplier");
            }
        }

        /// <summary>
        /// Restore supplier by id from trash.
        /// </summary>
        /// <param name="id">Supplier id</param>
        /// <returns>Result of restore process</returns>
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreSupplier(int id)
        {
            _logger.LogInformation("Restoring supplier with ID: {SupplierId}", id);

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID: {SupplierId} not found for restore", id);

                return NotFound();
            }

            if (!supplier.IsDeleted)
            {
                _logger.LogWarning("Supplier with ID: {SupplierId} is not in trash, cannot restore", id);

                return BadRequest("Supplier is not in trash");
            }

            _logger.LogInformation("Marking supplier with ID: {SupplierId} as not deleted", id);

            supplier.IsDeleted = false;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Supplier with ID: {SupplierId} restored successfully", id);

                return NoContent();
            }
            else
            {
                _logger.LogError("Failed to restore supplier with ID: {SupplierId}", id);

                return BadRequest("Failed to restore supplier");
            }
        }

        /// <summary>
        /// Permanently delete supplier by id. 
        /// The supplier must be in trash (marked as deleted) before it can be permanently deleted.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Result of permanent delete process</returns>
        [HttpDelete("{id}/permanent-delete")]
        public async Task<IActionResult> PermanentDeleteSupplier(int id)
        {
            _logger.LogInformation("Permanently deleting supplier with ID: {SupplierId}", id);

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID: {SupplierId} not found for permanent delete", id);

                return NotFound();
            }

            if (!supplier.IsDeleted)
            {
                _logger.LogWarning("Supplier with ID: {SupplierId} is not in trash, cannot permanently delete", id);

                return BadRequest("Supplier must be in trash before permanent delete");
            }

            _logger.LogInformation("Removing supplier with ID: {SupplierId} from database context", id);

            _context.Suppliers.Remove(supplier);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Supplier with ID: {SupplierId} permanently deleted successfully", id);

                return NoContent();
            }
            else
            {
                _logger.LogError("Failed to permanently delete supplier with ID: {SupplierId}", id);

                return BadRequest("Failed to permanently delete supplier");
            }
        }
    }
}
