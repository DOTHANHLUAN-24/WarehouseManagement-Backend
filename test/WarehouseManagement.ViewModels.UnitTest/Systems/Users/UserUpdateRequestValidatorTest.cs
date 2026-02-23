using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.ViewModels.UnitTest.Systems.Users
{
    public class UserUpdateRequestValidatorTest
    {
        private UserUpdateRequestValidator _validator;
        private UserUpdateRequest _request;

        public UserUpdateRequestValidatorTest()
        {
            _validator = new UserUpdateRequestValidator();
            _request = new UserUpdateRequest()
            {
                FirstName = "Shuhy",
                LastName = "Daval"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_First_Name(string data)
        {
            _request.FirstName = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Last_Name(string data)
        {
            _request.LastName = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Valid_When_Request_Is_Valid()
        {
            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_First_Name_Over_Limit()
        {
            _request.FirstName = new string('a', 51);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Last_Name_Over_Limit()
        {
            _request.FirstName = new string('a', 51);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }
    }
}