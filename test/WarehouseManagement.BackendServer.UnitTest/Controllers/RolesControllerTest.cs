using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WarehouseManagement.BackendServer.Controllers;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.ViewModels.Systems;
using WarehouseManagement.ViewModels.Systems.Roles;

namespace WarehouseManagement.BackendServer.UnitTest.Controllers
{
    public class RolesControllerTest
    {
        private readonly InMemoryContextFactory _factory = new();
        private readonly Mock<ILogger<RolesController>> _mockLogger = new Mock<ILogger<RolesController>>();

        // =========================
        // Helpers
        // =========================

        private Mock<RoleManager<IdentityRole>> CreateMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(
                store.Object,
                null!, // IRoleValidator
                null!, // ILookupNormalizer
                null!, // IdentityErrorDescriber
                null!  // ILogger
            );
        }

        private RoleManager<IdentityRole> CreateRealRoleManager(ApplicationDbContext context)
        {
            var store = new RoleStore<IdentityRole, ApplicationDbContext>(context);

            var roleValidators = new List<IRoleValidator<IdentityRole>>
            {
                new RoleValidator<IdentityRole>()
            };

            var normalizer = new UpperInvariantLookupNormalizer();
            var errorDescriber = new IdentityErrorDescriber();
            var logger = new Mock<ILogger<RoleManager<IdentityRole>>>().Object;

            return new RoleManager<IdentityRole>(
                store,
                roleValidators,
                normalizer,
                errorDescriber,
                logger
            );
        }


        // =========================
        // Constructor
        // =========================

        [Fact]
        public void ShouldCreateInstance_NotNull_ReturnSuccess()
        {
            var mockRoleManager = CreateMockRoleManager();
            var controller = new RolesController(mockRoleManager.Object, _mockLogger.Object);

            Assert.NotNull(controller);
        }

        // =========================
        // Post role
        // =========================

        [Theory]
        [InlineData("test", "test")]
        [InlineData("test", "testkdakljd")]
        [InlineData("testdklajkadjj", "test")]
        public async Task PostRole_ValidInput_Success(string id, string name)
        {
            // Arrange
            var mockRoleManager = CreateMockRoleManager();
            mockRoleManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new RolesController(mockRoleManager.Object, _mockLogger.Object);

            // Act
            var result = await controller.PostRole(new RoleCreateRequest
            {
                Id = id,
                Name = name
            });

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task PostRole_CreateFailed_ReturnBadRequest()
        {
            // Arrange
            var mockRoleManager = CreateMockRoleManager();
            mockRoleManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed());

            var controller = new RolesController(mockRoleManager.Object, _mockLogger.Object);

            // Act
            var result = await controller.PostRole(new RoleCreateRequest
            {
                Id = "test",
                Name = "test"
            });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // =========================
        // Get role
        // =========================

        [Fact]
        public async Task GetRoles_HasData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            context.Roles.AddRange(
                new IdentityRole("role test 1"),
                new IdentityRole("role test 2"),
                new IdentityRole("role test 3")
            );
            await context.SaveChangesAsync();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.GetAllRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roles = Assert.IsAssignableFrom<IEnumerable<RoleViewModel>>(okResult.Value);

            Assert.Equal(3, roles.Count());
        }

        [Fact]
        public async Task GetRoles_HasNoData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.GetAllRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roles = Assert.IsAssignableFrom<IEnumerable<RoleViewModel>>(okResult.Value);

            Assert.Empty(roles);
        }

        // =========================
        // Get by id
        // =========================

        [Fact]
        public async Task GetById_HasData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            context.Roles.Add(new IdentityRole
            {
                Id = "Test1",
                Name = "Test1"
            });
            await context.SaveChangesAsync();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.GetById("Test1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roleViewModel = Assert.IsType<RoleViewModel>(okResult.Value);

            Assert.Equal("Test1", roleViewModel.Name);
        }


        [Fact]
        public async Task GetById_HasNoData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            context.Roles.Add(new IdentityRole
            {
                Id = "Test1",
                Name = "Test1"
            });
            await context.SaveChangesAsync();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.GetById("count");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Get role paging
        // =========================

        [Theory]
        [InlineData(null, 1, 10, 4)]
        [InlineData("Test", 1, 5, 2)]
        [InlineData("tad", 1, 10, 1)]
        [InlineData("data", 1, 10, 0)]
        public async Task GetRolesPaging_HasData_ReturnSuccess
            (
                string? filter,
                int pageIndex,
                int pageSize,
                int countItem
            )
        {
            // Arrange
            var context = _factory.Create();

            context.AddRange(
                new IdentityRole
                {
                    Id = "Test 1",
                    Name = "Test 1"
                },
                new IdentityRole
                {
                    Id = "Test 2",
                    Name = "Test 2"
                },
                new IdentityRole
                {
                    Id = "Random",
                    Name = "Random"
                },
                new IdentityRole
                {
                    Id = "tadklj",
                    Name = "dakjl"
                }
            );
            await context.SaveChangesAsync();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.GetRolesPaging(filter, pageIndex, pageSize);
            var resultOk = Assert.IsType<OkObjectResult>(result);
            var pagination = resultOk.Value as Pagination<RoleViewModel>;

            // Assert
            Assert.Equal(countItem, pagination!.TotalRecords);
        }

        [Fact]
        public async Task GetRolesPaging_HasNoData_ReturnNull()
        {
            // Arrange
            var context = _factory.Create();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.GetRolesPaging(null, 1, 10);
            var resultOk = Assert.IsType<OkObjectResult>(result);
            var pagination = resultOk.Value as Pagination<RoleViewModel>;

            // Assert
            Assert.Empty(pagination!.Items);
        }

        // =========================
        // Put role
        // =========================

        [Fact]
        public async Task PutRole_ValidInput_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            context.Add(
                new IdentityRole
                {
                    Id = "test",
                    Name = "desc test"
                }
            );
            await context.SaveChangesAsync();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.PutRole("test", new RoleUpdateRequest
            {
                Id = "test",
                Name="test"
            });

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutRole_HasNoData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.PutRole("test", new RoleUpdateRequest
            {
                Id = "test",
                Name = "45d45a6"
            });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // Delete role
        // =========================

        [Fact]
        public async Task DeleteRole_HasData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            context.AddRange(
                new IdentityRole
                {
                    Id = "Test 1",
                    Name = "Test 1"
                },
                new IdentityRole
                {
                    Id = "Test 2",
                    Name = "Test 2"
                },
                new IdentityRole
                {
                    Id = "Random",
                    Name = "Random"
                }
            );
            await context.SaveChangesAsync();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.DeleteRole("Test 1");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRole_HasNoData_ReturnSuccess()
        {
            // Arrange
            var context = _factory.Create();

            var roleManager = CreateRealRoleManager(context);
            var controller = new RolesController(roleManager, _mockLogger.Object);

            // Act
            var result = await controller.DeleteRole("Test 1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
