using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.BackendServer.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Get user view model by id.
        /// </summary>
        Task<UserViewModel?> GetUserByIdAsync(string id);

        /// <summary>
        /// Get all users as view models.
        /// </summary>
        Task<List<UserViewModel>> GetAllUsersAsync();

        /// <summary>
        /// Create a new user with password.
        /// Returns created user view model or null if failed.
        /// </summary>
        Task<UserViewModel?> CreateUserAsync(UserCreateRequest request);

        /// <summary>
        /// Update an existing user by id.
        /// Returns true when update succeeded.
        /// </summary>
        Task<bool> UpdateUserAsync(UserUpdateRequest request);

        /// <summary>
        /// Delete a user by id.
        /// Returns true when deletion succeeded.
        /// </summary>
        Task<bool> DeleteUserAsync(string id);
    }
}