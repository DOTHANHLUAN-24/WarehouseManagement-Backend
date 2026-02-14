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
            var listPermission = okResult as IEnumerable<PermissionInRoleViewModel>;

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
            var listPermission = okResult as IEnumerable<PermissionInRoleViewModel>;

            Assert.Null(listPermission!);
        }
    }
}
