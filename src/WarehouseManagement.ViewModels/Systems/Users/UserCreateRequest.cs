using System.Collections.Generic;

namespace WarehouseManagement.ViewModels.Systems.User
{
    public class UserCreateRequest: UserBase
    {
        public string Password { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
