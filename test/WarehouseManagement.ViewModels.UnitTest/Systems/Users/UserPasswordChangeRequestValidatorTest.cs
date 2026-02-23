using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.ViewModels.UnitTest.Systems.Users
{
    public class UserPasswordChangeRequestValidatorTest
    {
        private UserPasswordChangeRequestValidator _validator;
        private UserPasswordChangeRequest _request;

        public UserPasswordChangeRequestValidatorTest()
        {
            _validator = new UserPasswordChangeRequestValidator();
            _request = new UserPasswordChangeRequest()
            {
                UserId = "1234567890",
                CurrentPassword = "Kladj@123",
                NewPassword = "A5a4ad4A#!"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_User_Id(string data)
        {
            _request.UserId = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Current_Password(string data)
        {
            _request.CurrentPassword = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_New_Password(string data)
        {
            _request.NewPassword = data;

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
        public void Should_Return_Error_When_Invalid_Current_Password(string data)
        {
            _request.CurrentPassword = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("Kladj@123")]
        [InlineData("A5a4ad4A#!")]
        [InlineData("a54aA4!#A")]
        public void Should_Valid_When_Valid_Current_Password(string data)
        {
            _request.CurrentPassword = data;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
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
        public void Should_Return_Error_When_Invalid_New_Password(string data)
        {
            _request.NewPassword = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("Kladj@123")]
        [InlineData("A5a4ad4A#!")]
        [InlineData("a54aA4!#A")]
        public void Should_Valid_When_Valid_New_Password(string data)
        {
            _request.NewPassword = data;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }
    }
}
