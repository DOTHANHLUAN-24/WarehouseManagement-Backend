using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.ViewModels.Systems;
using WarehouseManagement.ViewModels.Systems.Roles;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(RoleManager<IdentityRole> _roleManager, ILogger<RolesController> _logger) : BaseController
    {
        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="roleViewModel">Role model</param>
        /// <returns>Results of the add process</returns>
        [HttpPost]
        public async Task<IActionResult> PostRole(RoleCreateRequest request)
        {
            _logger.LogInformation("Creating a new role with name: {RoleName}", request.Name);

            if (await _roleManager.RoleExistsAsync(request.Name))
            {
                _logger.LogWarning("Role with name {RoleName} already exists", request.Name);

                return BadRequest("Role already exists");
            }

            var role = new IdentityRole()
            {
                Id = request.Id,
                Name = request.Name,
                NormalizedName = request.Name.ToUpper()
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role with name {RoleName} created successfully", request.Name);

                return CreatedAtAction(nameof(GetById), new { id = role.Id }, request);
            }
            else
            {
                _logger.LogInformation("Failed to create role with name {RoleName}. Errors: {Errors}", request.Name, string.Join(", ", result.Errors.Select(e => e.Description)));

                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Get a role by id
        /// </summary>
        /// <param name="id">Role id</param>
        /// <returns>The role with the id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            _logger.LogInformation("Getting role with id: {RoleId}", id);

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role with id {RoleId} not found", id);
            
                return NotFound();
            }

            var roleVM = new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name!
            };

            _logger.LogInformation("Role with id {RoleId} retrieved successfully", id);

            return Ok(roleVM);
        }

        /// <summary>
        /// Get all roles in the system
        /// </summary>
        /// <returns>List of roles</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            _logger.LogInformation("Getting all roles");

            var roles = await _roleManager.Roles
                .Select(r => new RoleViewModel()
                {
                    Id = r.Id,
                    Name = r.Name!
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {RoleCount} roles", roles.Count);

            return Ok(roles);
        }

        /// <summary>
        /// Get paged roles filtered by id or name.
        /// </summary>
        /// <param name="filter">Search keyword</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns>Roles list filtered by keyword</returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetRolesPaging(string? filter, int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Getting paged roles with filter: {Filter}, pageIndex: {PageIndex}, pageSize: {PageSize}", filter, pageIndex, pageSize);

            if (pageIndex <= 0)
            {
                _logger.LogWarning("Invalid pageIndex: {PageIndex}. Resetting to 1.", pageIndex);

                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                _logger.LogWarning("Invalid pageSize: {PageSize}. Resetting to 10.", pageSize);

                pageSize = 10;
            }

            var query = _roleManager.Roles;

            if (!string.IsNullOrEmpty(filter))
            {
                _logger.LogInformation("Applying filter to roles query: {Filter}", filter);

                query = query.Where(x => x.Id.Contains(filter) || x.Name!.Contains(filter));
            }

            var totalRecords = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoleViewModel()
                {
                    Id = r.Id,
                    Name = r.Name!,
                }).ToListAsync();

            var pagination = new Pagination<RoleViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            };

            _logger.LogInformation("Retrieved {ItemCount} roles with total records: {TotalRecords}", items.Count, totalRecords);

            return Ok(pagination);
        }

        /// <summary>
        /// Update a role by id
        /// </summary>
        /// <param name="id">Role id</param>
        /// <param name="roleViewModel">Role model</param>
        /// <returns>Results of the update process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(string id, [FromBody] RoleUpdateRequest request)
        {
            _logger.LogInformation("Updating role with id: {RoleId}", id);

            if (id != request.Id)
            {
                _logger.LogWarning("Role id in the URL ({UrlId}) does not match role id in the body ({BodyId})", id, request.Id);
            
                return BadRequest();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role with id {RoleId} not found", id);

                return NotFound();
            }

            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpper();

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role with id {RoleId} updated successfully", id);

                return NoContent();
            }

            _logger.LogInformation("Failed to update role with id {RoleId}. Errors: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Delete a role by id.
        /// </summary>
        /// <param name="id">Role id</param>
        /// <returns>Results of the delete process</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            _logger.LogInformation("Deleting role with id: {RoleId}", id);

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role with id {RoleId} not found", id);
            
                return NotFound();
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role with id {RoleId} deleted successfully", id);

                var roleViewModel = new RoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name!
                };

                _logger.LogInformation("Returning deleted role with id {RoleId} in response", id);
                return Ok(roleViewModel);
            }

            _logger.LogInformation("Failed to delete role with id {RoleId}. Errors: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));

            return BadRequest(result.Errors);
        }
    }
}
