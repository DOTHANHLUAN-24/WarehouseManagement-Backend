using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WarehouseManagement.BackendServer.Controllers;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;

namespace WarehouseManagement.BackendServer.UnitTest.Controllers
{
    public class UsersControllerTest
    {
        private readonly InMemoryContextFactory _factory = new ();

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

            return new UserManager<User>(
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

    }
}
