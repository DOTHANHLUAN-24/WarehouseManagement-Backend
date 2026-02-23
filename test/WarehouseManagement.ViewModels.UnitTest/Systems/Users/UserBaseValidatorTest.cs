using System.Runtime.CompilerServices;
using WarehouseManagement.ViewModels.Systems.User;

namespace WarehouseManagement.ViewModels.UnitTest.Systems.Users
{
    public class UserBaseValidatorTest
    {
        private UserBaseValidator<UserBase> _validator;
        private UserBase _request;

        public UserBaseValidatorTest()
        {
            _validator = new UserBaseValidator<UserBase>();
            _request = new UserBase()
            {
                UserName = "Test",
                Email = "example@domain.com",
                PhoneNumber = "0123456789",
                FirstName = "Shuhy",
                LastName = "Daval"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_User_Name(string data)
        {
            _request.UserName = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Email(string data)
        {
            _request.Email = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Phone_Number(string data)
        {
            _request.PhoneNumber = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
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

        [Theory]
        [InlineData("dkhaljald")]
        [InlineData("adhkdahl@jdklajl")]
        [InlineData("adhkdahljdklajl.akdjl")]
        [InlineData(".akdjl")]
        [InlineData("@dakla.akdjl")]
        [InlineData("dakjdalj@kadkad.")]
        public void Should_Error_When_In_Correct_Format_For_Email(string data)
        {
            _request.Email = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("05a4")]
        [InlineData("445656")]
        [InlineData("445656+*")]
        [InlineData("23A*da+")]
        [InlineData("+84513")]
        [InlineData("+84513ad")]
        [InlineData("+84513378#!")]
        [InlineData("+84513ad+")]
        public void Should_Error_When_Incorrect_Format_For_Phone_Number(string data)
        {
            _request.PhoneNumber = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("a@gmail.ocm", "+84046542597","tets", "ajkdh", "dijklad")]
        [InlineData("akald@gmail.ocm", "0046542197","dlaa", "ajkdh", "dijklad")]
        public void Should_Valid_When_Valid_Input(
            string email,
            string phoneNumber,
            string firstName,
            string lastName,
            string userName)
        {
            _request.Email = email;
            _request.PhoneNumber = phoneNumber;
            _request.FirstName = firstName;
            _request.LastName = lastName;
            _request.UserName = userName;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_First_Name_Too_Long()
        {
            _request.FirstName = new string('a', 51);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }
        
        [Fact]
        public void Should_Return_Error_When_Last_Name_Too_Long()
        {
            _request.FirstName = new string('a', 51);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }
    }
}
