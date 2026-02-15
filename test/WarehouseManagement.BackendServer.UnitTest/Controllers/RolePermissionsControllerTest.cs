using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Controllers;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Systems.Permissions;

namespace WarehouseManagement.BackendServer.UnitTest.Controllers
{
    public class RolePermissionsControllerTest
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionsControllerTest()
        {
            _context = new InMemoryContextFactory().Create();
        }

        // =========================
        // Constructor
        // =========================

        [Fact]
        public void ShouldCreateInstance_NotNull_ReturnSuccess()
        {
            var controller = new RolePermissionsController(_context);

            Assert.NotNull(controller);
        }

        // =========================
        // Get permission by role
        // =========================

        [Fact]
        public async Task GetPermissionByRoleId_HasData_Success()
        {
            // Arrange
            _context.Permissions.AddRange(
                new Permission
                ("test 1", "VIEW")
                );

            _context.RolePermissions.AddRange
            (
                new RolePermission
                {
                    RoleId = "test role 1",
                    PermissionId = 1
                },
                new RolePermission
                {
                    RoleId = "test role 2",
                    PermissionId = 1
                },
                new RolePermission
                {
                    RoleId = "test role 3",
                    PermissionId = 1
                }
            );

            await _context.SaveChangesAsync();

            var controller = new RolePermissionsController(_context);

            // Act
            var result = await controller.GetPermissionByRoleId("test role 1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listPermission = Assert.IsAssignableFrom<IEnumerable<PermissionInRoleViewModel>>(okResult.Value);

            Assert.Single(listPermission!);
        }

        [Fact]
        public async Task GetPermissionByRoleId_HasNoData_Success()
        {
            // Arrange
            var controller = new RolePermissionsController(_context);

            // Act
            var result = await controller.GetPermissionByRoleId("test role 1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listPermission = Assert.IsAssignableFrom<IEnumerable<PermissionInRoleViewModel>>(okResult.Value);

            Assert.Empty(listPermission!);
        }

        // =========================
        // Put permission by role
        // =========================

        [Fact]
        public async Task PutPermissionByRoleId_ValidInput_Success()
        {
            // Arrange
            _context.Permissions.AddRange(
                new Permission
                ("test 1", "VIEW")
                );

            await _context.SaveChangesAsync();

            var controller = new RolePermissionsController(_context);

            // Act
            var result = await controller.PutPermissionByRoleId("test role 1", new UpdatePermissionRequest
            {
                Permissions = new List<PermissionViewModel>
                {
                    new PermissionViewModel
                    {
                        FunctionId = "test function 1",
                        Action = "VIEW"
                    },
                    new PermissionViewModel
                    {
                        FunctionId = "test function 2",
                        Action = "VIEW"
                    },
                    new PermissionViewModel
                    {
                        FunctionId = "test function 3",
                        Action = "VIEW"
                    },
                }
            });

            var okResult = Assert.IsType<OkResult>(result);

            // Assert
            Assert.NotNull(result);
        }
    }
}
