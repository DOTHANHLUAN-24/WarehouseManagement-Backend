namespace WarehouseManagement.ViewModels.Systems.Login
{
    public class LoginResponseModel
    {
        public string? UserName { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
    