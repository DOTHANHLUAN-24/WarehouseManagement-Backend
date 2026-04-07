using WarehouseManagement.BackendServer.Data.Entities;

namespace WarehouseManagement.BackendServer.Repositories.Interfaces.Authentication
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmail(string email);

        Task<User?> GetUserById(string id);

        Task<List<User>> GetAllUsers();

        Task<bool> UpdateUser(string userId, User user);

        Task<bool> DeleteUser(string userId);
    }
}
