using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Systems.User
{
    /// <summary>
    /// Request to assign a single role to a user.
    /// </summary>
    public class UserRoleAssignRequest
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}