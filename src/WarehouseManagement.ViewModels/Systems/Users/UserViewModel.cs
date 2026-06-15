using System.Collections.Generic;

namespace WarehouseManagement.ViewModels.Systems.User
{
    public class UserViewModel : UserBase
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
