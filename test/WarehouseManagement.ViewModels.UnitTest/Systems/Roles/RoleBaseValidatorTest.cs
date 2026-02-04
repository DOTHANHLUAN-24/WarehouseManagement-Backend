using WarehouseManagement.ViewModels.Systems.Role;

namespace WarehouseManagement.ViewModels.UnitTest.Systems.Roles
{
    public class RoleBaseValidatorTest
    {
        private readonly RoleBaseValidator<RoleBase> _validator;
        private readonly RoleBase _request;

        public RoleBaseValidatorTest()
        {
            _validator = new RoleBaseValidator<RoleBase>();
            _request = new RoleBase
            {
                Id = "admin",
                Name = "admin"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Role_Id(string data)
        {
            _request.Id = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Role_Name(string data)
        {
            _request.Name = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Request_Role_Empty(string data)
        {
            _request.Name = data;
            _request.Id = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("akdhhahdlhal", "kjakdjkladjladj")]
        [InlineData("dak", "djhadlah")]
        [InlineData("kdahdkkjhadkhadjk", "dhalhlkadh")]
        public void Should_Valid_When_Valid_Input(string name, string id) {
            _request.Name = name;
            _request.Id = id;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }
    }
}
