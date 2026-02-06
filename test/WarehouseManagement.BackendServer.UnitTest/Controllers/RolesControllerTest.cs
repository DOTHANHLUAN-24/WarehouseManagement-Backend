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
        private List<IdentityRole> _roleSource = new List<IdentityRole>()
        {
            new IdentityRole("role test 1"),
            new IdentityRole("role test 2"),
            new IdentityRole("role test 3"),
            new IdentityRole("role test 4"),
            new IdentityRole("role test 5"),
        };


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

        
    }
}
