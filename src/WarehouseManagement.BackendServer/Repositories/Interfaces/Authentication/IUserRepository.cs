using WarehouseManagement.BackendServer.Data.Entities;

namespace WarehouseManagement.BackendServer.Repositories.Interfaces.Authentication
{
    public interface IUserRepository
    {
        // Find user by email address
        Task<User?> GetUserByEmail(string email);

        // Find user by id
        Task<User?> GetUserById(string id);

        // List all users
        Task<List<User>> GetAllUsers();

        // Create a new user with password (returns created User on success)
        Task<User?> CreateUser(User user, string password);

        // Update existing user by id - returns true if update succeeded
        Task<bool> UpdateUser(string userId, User user);

        // Delete user by id - returns true if deletion succeeded
        Task<bool> DeleteUser(string userId);   
    }
}