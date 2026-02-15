using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Systems.Permissions;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionsController(ApplicationDbContext _context) : BaseController
    {
        /// <summary>
        /// Get all permissions by role id
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <returns>List of permissions</returns>
        [HttpGet("roles/{roleId}")]
        public async Task<IActionResult> GetPermissionByRoleId(string roleId)
        {
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => new PermissionInRoleViewModel
                {
                    RoleId = rp.RoleId,
                    PermissionId = rp.PermissionId
                })
                .ToListAsync();

            return Ok(permissions);
        }

        /// <summary>
        /// Replace all permissions of a role
        /// </summary>
        /// <param name="roleId">Role id</param>
        /// <param name="request">List of permissions to assign</param>
        /// <returns>Update result</returns>
        [HttpPut("roles/{roleId}/permissions")]
        public async Task<IActionResult> PutPermissionByRoleId(string roleId, [FromBody] UpdatePermissionRequest request)
        {
            var existingPermissions = await _context.Permissions.ToListAsync();

            var permissionIds = new List<int>();

            foreach (var p in request.Permissions)
            {
                var permission = existingPermissions.FirstOrDefault(x =>
                    x.FunctionId == p.FunctionId &&
                    x.Action == p.Action);

                if (permission == null)
                {
                    permission = new Permission(p.FunctionId, p.Action);
                    _context.Permissions.Add(permission);
                    await _context.SaveChangesAsync();
                }

                permissionIds.Add(permission.Id);
            }

            var oldRolePermissions = _context.RolePermissions
                .Where(x => x.RoleId == roleId);

            _context.RolePermissions.RemoveRange(oldRolePermissions);

            var newRolePermissions = permissionIds
                .Distinct()
                .Select(pid => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = pid
                });

            await _context.RolePermissions.AddRangeAsync(newRolePermissions);
            
            var result =  await _context.SaveChangesAsync();

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }

    }
}
