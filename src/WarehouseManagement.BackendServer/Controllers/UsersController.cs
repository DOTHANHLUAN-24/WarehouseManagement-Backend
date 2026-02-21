using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Systems;
using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(UserManager<User> _userManager, ILogger<UsersController> _logger) : BaseController
    {
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostUser(UserCreateRequest request)
        {
            _logger.LogInformation("Creating a new user with email: {Email}", request.Email);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.UserName,
                LastName = request.LastName,
                FirstName = request.FirstName,
                PhoneNumber = request.PhoneNumber
            };

            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("User with email {Email} already exists", user.Email);

                return BadRequest("The email you use already exists in the database.");
            }


            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with email {Email} created successfully", user.Email);

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, request);
            }
            else
            {
                _logger.LogError("Failed to create user with email {Email}. Errors: {Errors}", user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

                return BadRequest("Failed to create a user");
            }
        }

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>The user with the id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            _logger.LogInformation("Getting user with id: {UserId}", id);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", id);

                return NotFound("Can't found the user.");
            }

            var userVM = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            _logger.LogInformation("User with id {UserId} retrieved successfully", id);

            return Ok(userVM);
        }

        /// <summary>
        /// Get all users in the system
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Getting all users");

            var users = await _userManager.Users
                .Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Email = u.Email!, 
                    PhoneNumber = u.PhoneNumber!,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName!
                })
                .ToListAsync();

            _logger.LogInformation("Total users retrieved: {UserCount}", users.Count);

            return Ok(users);
        }

        /// <summary>
        /// Get paged users filtered by id or name
        /// </summary>
        /// <param name="filter">Search keyword</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns>Users list filtered by keyword</returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetUsersPaging(string? filter, int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("Getting users with filter: {Filter}, pageIndex: {PageIndex}, pageSize: {PageSize}", filter, pageIndex, pageSize);

            if (pageIndex <= 0)
            {
                _logger.LogWarning("Invalid pageIndex: {PageIndex}. Resetting to 1.", pageIndex);

                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                _logger.LogWarning("Invalid pageSize: {PageSize}. Resetting to 10.", pageSize);
            
                pageSize = 10;
            }

            var query = _userManager.Users;
            
            if (!string.IsNullOrEmpty(filter))
            {
                _logger.LogInformation("Applying filter to users query: {Filter}", filter);

                query = query.Where(x => x.Id.Contains(filter) || x.UserName!.Contains(filter));
            }

            var totalRecords = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Email = u.Email!,
                    PhoneNumber = u.PhoneNumber!,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName!
                }).ToListAsync();

            var pagination = new Pagination<UserViewModel>
            {
                Items = items,
                TotalRecords = totalRecords
            };

            _logger.LogInformation("Retrieved {UserCount} users with filter: {Filter}", items.Count, filter);

            return Ok(pagination);
        }

        /// <summary>
        /// Update a user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="request">User model</param>
        /// <returns>Results of the update process</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] UserUpdateRequest request)
        {
            _logger.LogInformation("Updating user with id: {UserId}", id);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", id);
            
                return NotFound("Can't found the user.");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with id {UserId} updated successfully", id);

                return NoContent();
            }

            _logger.LogError("Failed to update user with id {UserId}. Errors: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));

            return BadRequest();
        }

        /// <summary>
        /// Delete a user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Results of the delete process</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            _logger.LogInformation("Deleting user with id: {UserId}", id);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", id);
            
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with id {UserId} deleted successfully", id);

                var userVM = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                _logger.LogInformation("Deleted user with id {UserId} details: {@UserDetails}", id, userVM);

                return Ok(userVM);
            }

            _logger.LogError("Failed to delete user with id {UserId}. Errors: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));

            return BadRequest("Failed to deleted the user");
        }
    }
}
