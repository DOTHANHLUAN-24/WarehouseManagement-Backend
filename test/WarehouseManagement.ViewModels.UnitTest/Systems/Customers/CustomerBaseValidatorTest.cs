using WarehouseManagement.ViewModels.Systems.Customers;

namespace WarehouseManagement.ViewModels.UnitTest.Systems.Customers
{
    public class CustomerBaseValidatorTest
    {
        private readonly CustomerBaseValidator<CustomerBase> _validator;
        private readonly CustomerBase _request;

        public CustomerBaseValidatorTest()
        {
            _validator = new CustomerBaseValidator<CustomerBase>();
            _request = new CustomerBase
            {
                FullName = "Customer 1",
                PhoneNumber = "+84123456789"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Full_Name(string data)
        {
            _request.FullName = data;
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
        [InlineData("djahalk", "0454481236")]
        [InlineData("djah", "+84454481236")]
        public void Should_Return_Error_When_Valid_Phone_Number(string fullName, string phoneNumber)
        {
            _request.FullName = fullName;
            _request.PhoneNumber = phoneNumber;
            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("djahalk", "12345â6789")]
        [InlineData("djah", "+8412345ldka67890")]
        [InlineData("dkajdkal", "+841890")]
        public void Should_Return_Error_When_Invalid_Phone_Number(string fullName, string phoneNumber)
        {
            _request.FullName = fullName;
            _request.PhoneNumber = phoneNumber;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }
    }
}