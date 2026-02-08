using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("RolePermissions")]
    public class RolePermission
    {
        public RolePermission(string roleId, int permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }

        public RolePermission() { }

        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public int PermissionId { get; set; }
    }
}
