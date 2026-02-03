namespace WarehouseManagement.ViewModels.Systems.User
{
    public class UserCreateRequest: UserBase
    {
        public string Password { get; set; } = string.Empty;
    }
}
