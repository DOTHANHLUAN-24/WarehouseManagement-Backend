using WarehouseManagement.ViewModels.Contents.Warehouses;

namespace WarehouseManagement.ViewModels.UnitTest.Contents.Warehouses
{
    public class WarehouseBaseValidatorTest
    {
        private readonly WarehouseBaseValidator<WarehouseBase> _validator;
        private readonly WarehouseBase _request;

        public WarehouseBaseValidatorTest()
        {
            _validator = new WarehouseBaseValidator<WarehouseBase>();
            _request = new WarehouseBase
            {
                Location = "Hà Nội",
                Capacity = 1,
                Email = "test@gmail.com",
            };
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Return_Error_When_Missing_Location(string data)
        {
            _request.Location = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Return_Error_When_Missing_Email(string data)
        {
            _request.Email = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Location_Max_Length()
        {
            _request.Location = new string('a', 201);

            var result = _validator.Validate(_request); 
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Return_Error_When_Capacity_Invalid(int data)
        {
            _request.Capacity = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("testemail.cajkla")]
        [InlineData("testemail@.cajkla")]
        [InlineData("testemail@cajkla")]
        [InlineData("testemailcajkla")]
        [InlineData("testemai.")]
        [InlineData(".testemai")]
        [InlineData("@testemai")]
        [InlineData("@.testemai")]
        [InlineData("testemai@")]
        [InlineData("testemai@.")]
        public void Should_Return_Error_When_Email_Invalid(string data)
        {
            _request.Email = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Success_When_Valid_Request()
        {
            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("Test@gmail.com")]
        [InlineData("Test@gmakl.alm")]
        [InlineData("Tes893189t@gmakl.alm")]
        [InlineData("893189@gmakl.alm")]
        public void Should_Return_Error_When_Email_Valid(string data)
        {
            _request.Email = data;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }
    }
}
