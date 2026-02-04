using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WarehouseManagement.BackendServer.Controllers;
using WarehouseManagement.ViewModels.Systems.Role;

namespace WarehouseManagement.UnitTest.Controllers
{
    public class RolesControllerTest
    {
        // <Name method>_<Condition>_<Excepted results>
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;

        public RolesControllerTest()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null!, null!, null!, null!);
            var rolesController = new RolesController(_mockRoleManager.Object);
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_ReturnSuccess()
        {
            var rolesController = new RolesController(_mockRoleManager.Object);

            Assert.NotNull(rolesController);
        }

        [Theory]
        [InlineData("", "test")]
        [InlineData("test", "")]
        [InlineData("", "")]
        public void RoleCreateRequestValidator_Invalid_ReturnError(string id, string name)
        {
            var validator = new RoleCreateRequestValidator();

            var result = validator.Validate(new RoleCreateRequest
            {
                Id = id,
                Name = name
            });

            Assert.False(result.IsValid);
        }

    }
}
