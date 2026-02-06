using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Domain.Entities
{
    public class RolePermission
    {
        public RolePermission(string roleId, int permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }

        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public int PermissionId { get; set; }
    }
}
