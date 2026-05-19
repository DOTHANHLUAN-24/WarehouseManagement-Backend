using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Systems.User
{
    /// <summary>
    /// Request to assign multiple roles to a user.
    /// </summary>
    public class UserRolesAssignRequest
    {
        [Required]
        public List<string> Roles { get; set; } = new();
    }
}