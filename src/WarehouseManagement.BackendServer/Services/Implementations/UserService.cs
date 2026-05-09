using AutoMapper;
using Microsoft.Extensions.Logging;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.BackendServer.Repositories.Interfaces.Authentication;
using WarehouseManagement.BackendServer.Services.Interfaces;
using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.BackendServer.Services.Implementations
{
    /// <summary>
    /// Business logic for user management.
    /// This service uses IUserRepository to perform persistence operations and maps entities to DTOs.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserViewModel?> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null) return null;
            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsers();
            return _mapper.Map<List<UserViewModel>>(users);
        }

        public async Task<UserViewModel?> CreateUserAsync(UserCreateRequest request)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                LockoutEnabled = false,
                IsActive = true
            };

            var created = await _userRepository.CreateUser(user, request.Password);
            if (created == null)
            {
                _logger.LogWarning("User creation failed for username {UserName}", request.UserName);
                return null;
            }

            return _mapper.Map<UserViewModel>(created);
        }

        public async Task<bool> UpdateUserAsync(UserUpdateRequest request)
        {
            var user = new User
            {
                Id = request.Id,
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.IsActive
            };

            return await _userRepository.UpdateUser(request.Id, user);  
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            return await _userRepository.DeleteUser(id);
        }
    }
}