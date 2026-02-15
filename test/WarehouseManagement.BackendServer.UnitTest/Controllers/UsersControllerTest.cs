using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WarehouseManagement.BackendServer.Controllers;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Systems;
using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.BackendServer.UnitTest.Controllers
{
    public class UsersControllerTest
    {
        private readonly InMemoryContextFactory _factory = new();

        // =========================
        // Helpers
        // =========================

        private Mock<UserManager<User>> CreateMockUserManager()
        {
            var userStore = new Mock<IUserStore<User>>();

            return new Mock<UserManager<User>>(
                userStore.Object,
                null!, // IOptions<IdentityOptions>
                null!, // IPasswordHasher<User>
                null!, // IEnumerable<IUserValidator<User>>
                null!, // IEnumerable<IPasswordValidator<User>>
                null!, // ILookupNormalizer
                null!, // IdentityErrorDescriber
                null!, // IServiceProvider
                null!  // ILogger<UserManager<User>>
            );
        }

        private UserManager<User> CreateRealUserManager(ApplicationDbContext context)
        {
            var store = new UserStore<User, IdentityRole, ApplicationDbContext>(context);

            var options = Options.Create(new IdentityOptions());
            var passwordHasher = new PasswordHasher<User>();

            var userValidators = new List<IUserValidator<User>>
            {
                new UserValidator<User>()
            };

            var passwordValidators = new List<IPasswordValidator<User>>
            {
                new PasswordValidator<User>()
            };

            var normalizer = new UpperInvariantLookupNormalizer();
            var errorDescriber = new IdentityErrorDescriber();
            var services = new Mock<IServiceProvider>().Object;
            var logger = new Mock<ILogger<UserManager<User>>>().Object;

            return new UserManager<User>
            (
                store,
                options,
                passwordHasher,
                userValidators,
                passwordValidators,
                normalizer,
                errorDescriber,
                services,
                logger
            );
        }



        // =========================
        // Constructor
        // =========================

        [Fact]
        public void ShouldCreateInstance_NotNull_ReturnSuccess()
        {
            var mockUserManager = CreateMockUserManager();
            var controller = new UsersController(mockUserManager.Object);

            Assert.NotNull(controller);
        }

        // =========================
        // Post user
        // =========================

        [Fact]
        public async Task PostUser_ValidInput_ReturnSuccess()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            mockUserManager
              .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
              .ReturnsAsync(IdentityResult.Success);

            mockUserManager
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            mockUserManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new UsersController(mockUserManager.Object);

            var request = new UserCreateRequest
            {
                UserName = "testuser",
                Email = "test@gmail.com",
                Password = "Password@123",
                FirstName = "Thanh Luan",
                LastName = "Do",
                PhoneNumber = "0123456789"
            };

            // Act
            var result = await controller.PostUser(request);

            // Assert
            Assert.NotNull(result);

        }

        [Fact]
        public async Task PostUser_CreateFailed_ReturnBadRequest()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            mockUserManager
              .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
              .ReturnsAsync(IdentityResult.Failed());

            var controller = new UsersController(mockUserManager.Object);

            // Act
            var result = await controller.PostUser(new UserCreateRequest
            {
                UserName = "testuser",
                Email = "test@gmail.com",
                Password = "Password@123",
                FirstName = "Thanh Luan",
                LastName = "Do",
                PhoneNumber = "0123456789"
            });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // =========================
        // Get user
        // =========================

        [Fact]
        public async Task GetUsers_HasData_ReturnListUser()
        {
            // Arrange
            var context = _factory.Create();

            context.Users.AddRange
            (
                new User
                {
                    Id = "1",
                    UserName = "testuser1",
                    Email = "test1@gmail.com",
                    FirstName = "Test1",
                    LastName = "David",
                    PhoneNumber = "0123456787"
                },
                new User
                {
                    Id = "2",
                    UserName = "testuser2",
                    Email = "test2@gmail.com",
                    FirstName = "Test2",
                    LastName = "Bob",
                    PhoneNumber = "0123456788"
                }
            );
            await context.SaveChangesAsync();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userViewModels = okResult.Value as IEnumerable<UserViewModel>;

            Assert.Equal(2, userViewModels!.Count());
        }

        [Fact]
        public async Task GetUsers_HasNoData_ReturnListUser()
        {
            // Arrange
            var context = _factory.Create();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userViewModels = okResult.Value as IEnumerable<UserViewModel>;

            Assert.Empty(userViewModels!);
        }

        // =========================
        // Get by id
        // =========================

        [Fact]
        public async Task GetById_HasData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            context.Users.AddRange
            (
               new User
               {
                   Id = "1",
                   UserName = "testuser1",
                   Email = "test1@gmail.com",
                   FirstName = "Test1",
                   LastName = "David",
                   PhoneNumber = "0123456787"
               },
                new User
                {
                    Id = "2",
                    UserName = "testuser2",
                    Email = "test2@gmail.com",
                    FirstName = "Test2",
                    LastName = "Bob",
                    PhoneNumber = "0123456788"
                }
            );
            await context.SaveChangesAsync();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.GetById("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userViewModel = Assert.IsType<UserViewModel>(okResult.Value);

            Assert.Equal("testuser1", userViewModel.UserName);
            Assert.Equal("test1@gmail.com", userViewModel.Email);
        }


        [Fact]
        public async Task GetById_HasNoData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            context.Users.AddRange
            (
               new User
               {
                   Id = "1",
                   UserName = "testuser1",
                   Email = "test1@gmail.com",
                   FirstName = "Test1",
                   LastName = "David",
                   PhoneNumber = "0123456787"
               },
                new User
                {
                    Id = "2",
                    UserName = "testuser2",
                    Email = "test2@gmail.com",
                    FirstName = "Test2",
                    LastName = "Bob",
                    PhoneNumber = "0123456788"
                }
            );
            await context.SaveChangesAsync();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.GetById("count");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // =========================
        // Get user paging
        // =========================

        [Theory]
        [InlineData(null, 1, 10, 4)]
        [InlineData("test", 1, 10, 4)]
        [InlineData("1", 1, 10, 1)]
        [InlineData("data", 1, 10, 0)]
        public async Task GetUsersPaging_HasData_ReturnListUser
            (
                string? filter,
                int pageIndex,
                int pageSize,
                int countItem
            )
        {
            // Arrange
            var context = _factory.Create();

            context.Users.AddRange
            (
                new User
                {
                    Id = "1",
                    UserName = "testuser1",
                    Email = "test1@gmail.com",
                    FirstName = "Test1",
                    LastName = "David",
                    PhoneNumber = "0123456787"
                },
                new User
                {
                    Id = "2",
                    UserName = "testuser2",
                    Email = "test2@gmail.com",
                    FirstName = "Test2",
                    LastName = "Bob",
                    PhoneNumber = "0123456788"
                },
                new User
                {
                    Id = "3",
                    UserName = "testuser3",
                    Email = "test3@gmail.com",
                    FirstName = "Test3",
                    LastName = "Max",
                    PhoneNumber = "0223456788"
                },
                new User
                {
                    Id = "4",
                    UserName = "testuser4",
                    Email = "test4@gmail.com",
                    FirstName = "Test4",
                    LastName = "Shiny",
                    PhoneNumber = "0133456788"
                }
            );
            await context.SaveChangesAsync();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.GetUsersPaging(filter, pageIndex, pageSize);
            var resultOk = Assert.IsType<OkObjectResult>(result);
            var pagination = resultOk.Value as Pagination<UserViewModel>;

            // Assert
            Assert.Equal(countItem, pagination!.TotalRecords);
        }

        // =========================
        // Put user
        // =========================

        [Fact]
        public async Task PutUser_ValidInput_ReturnOkResult()
        {
            // Arrange
            var context = _factory.Create();

            context.Users.Add
            (
                new User
                {
                    Id = "1",
                    UserName = "testuser1",
                    Email = "test1@gmail.com",
                    FirstName = "Test1",
                    LastName = "David",
                    PhoneNumber = "0123456787"
                }
            );
            await context.SaveChangesAsync();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.PutUser("1", new UserUpdateRequest
            {
                FirstName = "update first name",
                LastName = "update last name"
            });

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutUser_HasNoData_ReturnOkResult()
        {
            // Arrange
            var context = _factory.Create();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.PutUser("1", new UserUpdateRequest
            {
                FirstName = "update first name",
                LastName = "update last name"
            });

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // =========================
        // Delete user
        // =========================

        [Fact]
        public async Task DeleteUser_HasData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            context.Users.AddRange
             (
                 new User
                 {
                     Id = "1",
                     UserName = "testuser1",
                     Email = "test1@gmail.com",
                     FirstName = "Test1",
                     LastName = "David",
                     PhoneNumber = "0123456787"
                 },
                 new User
                 {
                     Id = "2",
                     UserName = "testuser2",
                     Email = "test2@gmail.com",
                     FirstName = "Test2",
                     LastName = "Bob",
                     PhoneNumber = "0123456788"
                 },
                 new User
                 {
                     Id = "3",
                     UserName = "testuser3",
                     Email = "test3@gmail.com",
                     FirstName = "Test3",
                     LastName = "Max",
                     PhoneNumber = "0223456788"
                 },
                 new User
                 {
                     Id = "4",
                     UserName = "testuser4",
                     Email = "test4@gmail.com",
                     FirstName = "Test4",
                     LastName = "Shiny",
                     PhoneNumber = "0133456788"
                 }
             );
            await context.SaveChangesAsync();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.DeleteUser("1");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_HasNoData_ReturnNotFound()
        {
            // Arrange
            var context = _factory.Create();

            var userManager = CreateRealUserManager(context);
            var controller = new UsersController(userManager);

            // Act
            var result = await controller.DeleteUser("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
