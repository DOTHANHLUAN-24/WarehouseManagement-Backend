using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.ViewModels.Systems;
using WarehouseManagement.ViewModels.Systems.Role;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(RoleManager<IdentityRole> _roleManager) : ControllerBase
    {
        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="roleViewModel">Role model</param>
        /// <returns>Results of the add process</returns>
        [HttpPost]
        public async Task<IActionResult> PostRole(RoleCreateRequest request)
        {
            if (await _roleManager.RoleExistsAsync(request.Name))
                return BadRequest("Role already exists");

            var role = new IdentityRole()
            {
                Id = request.Id,
                Name = request.Name,
                NormalizedName = request.Name.ToUpper()
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetById), new { id = role.Id }, request);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Get a role by id
        /// </summary>
        /// <param name="id">Role id</param>
        /// <returns>The role with the Id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            
            var roleVM = new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name!
            };
            
            return Ok(roleVM);
        }

        /// <summary>
        /// Get all roles in the system.
        /// </summary>
        /// <returns>List of roles</returns>
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles
                .Select(r => new RoleViewModel()
                {
                    Id = r.Id,
                    Name = r.Name!
                })
                .ToListAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Get paged roles filtered by id or name.
        /// </summary>
        /// <param name="filter">Search keyword</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns></returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetRolesPaging(string? filter, int pageIndex, int pageSize)
        {
            var query = _roleManager.Roles;
           
            if (!string.IsNullOrEmpty(filter))
            {
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

            return Ok(pagination);
        }

        /// <summary>
        /// Update a role by id
        /// </summary>
        /// <param name="id">Role id</param>
        /// <param name="roleViewModel">Role model</param>
        /// <returns>Results of the update process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(string id, [FromBody] RoleCreateRequest request)
        {
            if (id != request.Id)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            
            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpper();
            
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return NoContent();
            }
            
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
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                var roleVM = new RoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name!
                };
                return Ok(roleVM);
            }
            
            return BadRequest(result.Errors);
        }
    }
}
