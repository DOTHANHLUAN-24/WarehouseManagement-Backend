using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.BackendServer.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserViewModel?> GetUserByIdAsync(string id);
    }
}
