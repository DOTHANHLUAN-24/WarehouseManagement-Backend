using FluentValidation.TestHelper;
using WarehouseManagement.ViewModels.Systems.Functions;

namespace WarehouseManagement.ViewModels.UnitTest.Systems.Functions
{
    public class FunctionBaseValidatorTest
    {
        private readonly FunctionBaseValidator<FunctionBase> _validator;
        private readonly FunctionBase _request;

        public FunctionBaseValidatorTest()
        {
            _validator = new FunctionBaseValidator<FunctionBase>();
            _request = new FunctionBase
            {
                Id = "function1",
                Name = "function1",
                Url = "/function1",
                SortOrder = 1,
                ParentId = null!,
                Icon = "icon-function1"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Function_Id(string data)
        {
            _request.Id = data;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Name(string data)
        {
            _request.Name = data;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Url(string data)
        {
            _request.Url = data;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("function", "functionName", "/function", 1, null, "icon")]
        [InlineData("function", "functionName", "/function", 1, "function1", "icon")]
        public void Should_Valid_When_Valid_Input(string id, string name, string url, int sortOrder, string? parentId, string icon)
        {
            _request.Id = id;
            _request.Name = name;
            _request.Url = url;
            _request.SortOrder = sortOrder;
            _request.ParentId = parentId!;
            _request.Icon = icon;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Id_Too_Long()
        {
            _request.Id = new string('a', 51);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Name_Too_Long()
        {
            _request.Name = new string('a', 201);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Url_Too_Long()
        {
            _request.Url = new string('a', 201);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Parent_Id_Too_Long()
        {
            _request.ParentId = new string('a', 51);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }
    }
}
