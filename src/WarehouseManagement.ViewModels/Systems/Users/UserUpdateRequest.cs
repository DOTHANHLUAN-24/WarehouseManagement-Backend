using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Systems.User
{
    public class UserUpdateRequest : UserBase
    {
        public string Id { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public List<string> Roles { get; set; } = new();
    }
}
