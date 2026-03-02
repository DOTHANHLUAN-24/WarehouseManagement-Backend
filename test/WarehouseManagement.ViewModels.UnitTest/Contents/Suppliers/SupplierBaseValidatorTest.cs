using WarehouseManagement.ViewModels.Contents.Suppliers;

namespace WarehouseManagement.ViewModels.UnitTest.Contents.Suppliers
{
    public class SupplierBaseValidatorTest
    {
        private readonly SupplierBaseValidator<SupplierBase> _validator;
        private readonly SupplierBase _request;

        public SupplierBaseValidatorTest()
        {
            _validator = new SupplierBaseValidator<SupplierBase>();
            _request = new SupplierBase
            {
                SupplierName = "Supplier 1",
                ContactPerson = "Contact Person 1",
                Phone = "0123456789",
                Address = "Address 1",
                Email = "test@gmail.com"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Supplier_Name(string data)
        {
            _request.SupplierName = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Phone(string data)
        {
            _request.Phone = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Address(string data)
        {
            _request.Address = data;

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
        [InlineData("123456789")]
        [InlineData("012345678901")]
        [InlineData("12345678901")]
        [InlineData("012345678a")]
        public void Should_Return_Error_When_Invalid_Phone(string data)
        {
            _request.Phone = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test@")]
        [InlineData("test@gmail")]
        public void Should_Return_Error_When_Invalid_Email(string data)
        {
            _request.Email = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Return_Valid_When_IsActive_Is_Valid(bool isActive)
        {
            _request.IsActive = isActive;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Return_Valid_When_IsDeleted_Is_Valid(bool isDeleted)
        {
            _request.IsDeleted = isDeleted;
            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Return_Success_When_Valid_Request()
        {
            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Contact_Person_Exceeds_Max_Length()
        {
            _request.ContactPerson = new string('a', 101);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Supplier_Name_Exceeds_Max_Length()
        {
            _request.SupplierName = new string('a', 201);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Return_Error_When_Address_Exceeds_Max_Length()
        {
            _request.Address = new string('a', 501);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }
    }
}
