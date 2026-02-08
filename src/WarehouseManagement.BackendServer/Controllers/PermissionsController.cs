using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WarehouseManagement.ViewModels.Systems.Permissions;

namespace WarehouseManagement.BackendServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController(IConfiguration _configuration) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetPermissionViews()
        {
            await using var connection =
                new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var sql = @"
                SELECT f.Id,
                       f.Name,
                       f.ParentId,
                       MAX(CASE WHEN p.Action = 'CREATE' THEN 1 ELSE 0 END) AS HasCreate,
                       MAX(CASE WHEN p.Action = 'UPDATE' THEN 1 ELSE 0 END) AS HasUpdate,
                       MAX(CASE WHEN p.Action = 'DELETE' THEN 1 ELSE 0 END) AS HasDelete,
                       MAX(CASE WHEN p.Action = 'VIEW'   THEN 1 ELSE 0 END) AS HasView
                FROM Functions f
                LEFT JOIN Permissions p ON p.FunctionId = f.Id
                GROUP BY f.Id, f.Name, f.ParentId
                ORDER BY f.ParentId;
            ";

            var result = await connection.QueryAsync<PermissionScreenViewModel>(sql);

            return Ok(result);
        }
    }
}
