
using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.ViewModels.UnitTest.Systems.Users
{
    public class UserCreateRequestValidatorTest
    {
        private UserCreateRequestValidator _validator;
        private UserCreateRequest _request;

        public UserCreateRequestValidatorTest()
        {
            _validator = new UserCreateRequestValidator();

            _request = new UserCreateRequest()
            {
                UserName = "Test",
                Email = "example@domain.com",
                PhoneNumber = "0123456789",
                FirstName = "Shuhy",
                LastName = "Daval",
                Password = "Kladj@123"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Password(string data)
        {
            _request.Password = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("12345678")]
        [InlineData("adjjkl")]
        [InlineData("a4d545a")]
        [InlineData("akAldakl")]
        [InlineData("A54321")]
        [InlineData("a5A4ad4A")]
        [InlineData("a54ad4!#")]
        [InlineData("544!#A")]
        [InlineData("54545#@!")]
        [InlineData("^@%!^%#")]
        [InlineData("A12#a")]
        public void Should_Return_Error_When_Invalid_Password(string data)
        {
            _request.Password = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("Kladj@123")]
        [InlineData("A5a4ad4A#!")]
        [InlineData("a54aA4!#A")]
        public void Should_Valid_When_Valid_Password(string data)
        {
            _request.Password = data;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }
    }
}
