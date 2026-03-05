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
        /// <summary>
        /// Get all warehouses that are not deleted
        /// </summary>
        /// <returns>List of warehouses</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllWarehouses()
        {
            _logger.LogInformation("Getting all warehouses");

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

            _logger.LogInformation("Total warehouses retrieved: {WarehouseCount}", listWarehouses.Count);

            return Ok(listWarehouses);
        }

        /// <summary>
        /// Create a new warehouse
        /// </summary>
        /// <param name="request">Ware house model</param>
        /// <returns>Result of create process</returns>
        [HttpPost]
        public async Task<IActionResult> PostWarehouse(WarehouseCreateRequest request)
        {
            _logger.LogInformation("Creating a new warehouse at location: {Location}", request.Location);

            var warehouse = new Warehouse
            {
                Location = request.Location,
                Capacity = request.Capacity,
                Email = request.Email,
                IsDeleted = request.IsDeleted
            };

            _context.Warehouses.Add(warehouse);

            _logger.LogInformation("Saving new warehouse to the database");

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Warehouse created successfully with ID: {WarehouseId}", warehouse.Id);

                return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse);
            }

            _logger.LogError("Failed to create warehouse at location: {Location}", request.Location);

            return BadRequest();
        }

        /// <summary>
        /// Get a warehouse by id
        /// </summary>
        /// <param name="id">Ware house id</param>
        /// <returns>Ware house with id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Getting warehouse by ID: {WarehouseId}", id);

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID: {WarehouseId} not found", id);

                return NotFound();
            }

            _logger.LogInformation("Warehouse with ID: {WarehouseId} retrieved successfully", id);

            var warehouseViewModel = new WarehouseViewModel
            {
                Id = warehouse.Id,
                Location = warehouse.Location,
                Capacity = warehouse.Capacity,
                Email = warehouse.Email,
                IsDeleted = warehouse.IsDeleted
            };

            _logger.LogInformation("Warehouse with ID: {WarehouseId} mapped to WarehouseViewModel successfully", id);

            return Ok(warehouseViewModel);
        }

        /// <summary>
        /// Get warehouses with filters and pagination
        /// </summary>
        /// <param name="location">Location of storage</param>
        /// <param name="capacity">Capacity of storage</param>
        /// <param name="email">Email of storage</param>
        /// <param name="filter">Name of storage</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List storage with filter</returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetWarehouses(
            [FromQuery] string? location,
            [FromQuery] int? capacity,
            [FromQuery] string? email,
            [FromQuery] string? filter,
            int pageIndex = 1,
            int pageSize = 10)
        {
            _logger.LogInformation("Getting warehouses with filters - Location: {Location}, Capacity: {Capacity}, Email: {Email}, Global Filter: {Filter}, PageIndex: {PageIndex}, PageSize: {PageSize}",
                location, capacity, email, filter, pageIndex, pageSize);

            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Warehouses
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // Filter by location
            if (!string.IsNullOrWhiteSpace(location))
            {
                _logger.LogInformation("Applying location filter: {Location}", location);

                query = query
                    .Where(x => x.Location.Contains(location));
            }

            // Filter by capacity
            if (capacity != null)
            {
                _logger.LogInformation("Applying capacity filter: {Capacity}", capacity);

                query = query
                    .Where(x => x.Capacity == capacity);
            }

            // Filter by email
            if (!string.IsNullOrWhiteSpace(email))
            {
                _logger.LogInformation("Applying email filter: {Email}", email);

                query = query
                    .Where(x => x.Email.Contains(email));
            }

            // Global filter
            if (!string.IsNullOrWhiteSpace(filter))
            {
                _logger.LogInformation("Applying global filter: {Filter}", filter);

                query = query
                    .Where(x => x.Location.Contains(filter) ||
                    x.Email.Contains(filter));
            }

            var totalRecords = await query.CountAsync();

            _logger.LogInformation("Total warehouses after filtering: {TotalRecords}", totalRecords);

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

            _logger.LogInformation("Total warehouses retrieved for current page: {ItemCount}", items.Count);

            var result = new Pagination<WarehouseViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            };

            _logger.LogInformation("Returning paginated result with {ItemCount} items and {TotalRecords} total records", items.Count, totalRecords);

            return Ok(result);
        }

        /// <summary>
        /// Update a warehouse by id
        /// </summary>
        /// <param name="id">Ware house id</param>
        /// <param name="request">Ware house model</param>
        /// <returns>Result of filter process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouse(int id, [FromBody] WarehouseUpdateRequest request)
        {
            _logger.LogInformation("Updating warehouse with ID: {WarehouseId}", id);

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID: {WarehouseId} not found", id);

                return NotFound();
            }

            _logger.LogInformation("Warehouse with ID: {WarehouseId} found. Updating details.", id);

            warehouse.Email = request.Email;
            warehouse.Location = request.Location;
            warehouse.Capacity = request.Capacity;

            var result = await _context.SaveChangesAsync();

            _logger.LogInformation("Warehouse with ID: {WarehouseId} update result: {Result}", id, result > 0 ? "Success" : "Failure");

            if (result > 0)
            {
                _logger.LogInformation("Warehouse with ID: {WarehouseId} updated successfully", id);

                return NoContent();
            }
            else
            {
                _logger.LogError("Failed to update warehouse with ID: {WarehouseId}", id);

                return BadRequest();
            }
        }

        /// <summary>
        /// Get warehouses in trash (soft deleted)
        /// </summary>
        /// <returns>List ware house in the trash</returns>
        [HttpGet("trash")]
        public async Task<IActionResult> GetWarehousesInTrash()
        {
            _logger.LogInformation("Getting warehouses in trash");

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

            _logger.LogInformation("Total warehouses in trash retrieved: {WarehouseCount}", warehousesInTrash.Count);

            return Ok(warehousesInTrash);
        }

        /// <summary>
        /// Soft delete a warehouse by id (move to trash)
        /// </summary>
        /// <param name="id">Ware house id</param>
        /// <returns>Result of soft process</returns>
        [HttpDelete("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteWarehouse(int id)
        {
            _logger.LogInformation("Soft deleting warehouse with ID: {WarehouseId}", id);

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID: {WarehouseId} not found", id);

                return NotFound();
            }

            if (warehouse.IsDeleted)
            {
                _logger.LogWarning("Warehouse with ID: {WarehouseId} is already in trash", id);

                return BadRequest();
            }

            _logger.LogInformation("Warehouse with ID: {WarehouseId} found. Marking as deleted.", id);

            warehouse.IsDeleted = true;

            _logger.LogInformation("Saving changes to soft delete warehouse with ID: {WarehouseId}", id);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Warehouse with ID: {WarehouseId} soft deleted successfully", id);

                return NoContent();
            }

            _logger.LogError("Failed to soft delete warehouse with ID: {WarehouseId}", id);

            return BadRequest();
        }

        /// <summary>
        /// Restore a warehouse from trash by id (undo soft delete)
        /// </summary>
        /// <param name="id">Ware house id</param>
        /// <returns>Result of restore process</returns>
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreWarehouse(int id)
        {
            _logger.LogInformation("Restoring warehouse with ID: {WarehouseId}", id);

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID: {WarehouseId} not found", id);

                return NotFound();
            }

            if (!warehouse.IsDeleted)
            {
                _logger.LogWarning("Warehouse with ID: {WarehouseId} is not in trash, cannot restore", id);

                return BadRequest();
            }

            _logger.LogInformation("Warehouse with ID: {WarehouseId} found in trash. Restoring.", id);

            warehouse.IsDeleted = false;

            _logger.LogInformation("Saving changes to restore warehouse with ID: {WarehouseId}", id);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Warehouse with ID: {WarehouseId} restored successfully", id);

                return NoContent();
            }

            _logger.LogError("Failed to restore warehouse with ID: {WarehouseId}", id);

            return BadRequest();
        }

        /// <summary>
        /// Permanently delete a warehouse by id (only if it's in trash)
        /// </summary>
        /// <param name="id">Ware house id</param>
        /// <returns>Process of permanent deleted</returns>
        [HttpDelete("{id}/permanent-delete")]
        public async Task<IActionResult> PermanentDeleteWarehouse(int id)
        {
            _logger.LogInformation("Permanently deleting warehouse with ID: {WarehouseId}", id);

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID: {WarehouseId} not found", id);

                return NotFound();
            }

            if (!warehouse.IsDeleted)
            {
                _logger.LogWarning("Warehouse with ID: {WarehouseId} is not in trash, cannot permanently delete", id);

                return BadRequest();
            }

            _logger.LogInformation("Warehouse with ID: {WarehouseId} found in trash. Permanently deleting.", id);

            _context.Warehouses.Remove(warehouse);

            _logger.LogInformation("Saving changes to permanently delete warehouse with ID: {WarehouseId}", id);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Warehouse with ID: {WarehouseId} permanently deleted successfully", id);

                return NoContent();
            }

            _logger.LogError("Failed to permanently delete warehouse with ID: {WarehouseId}", id);

            return BadRequest();
        }
    }
}
