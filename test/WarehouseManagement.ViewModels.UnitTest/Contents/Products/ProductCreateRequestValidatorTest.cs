using WarehouseManagement.ViewModels.Contents.Products;

namespace WarehouseManagement.ViewModels.UnitTest.Contents.Products
{

    public class ProductCreateRequestValidatorTest
    {
        private ProductCreateRequestValidator _validator;
        private ProductCreateRequest _request;

        public ProductCreateRequestValidatorTest()
        {
            _validator = new ProductCreateRequestValidator();

            _request = new ProductCreateRequest
            {
                Name = "Test Product",
                Description = "This is a test product.",
                CategoryId = 1,
                Code = "TP001",
                Price = 10.99m,
                InitialStock = 100,
                SKU = "TEST12345"
            };
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-1.5)]
        public void Should_Have_Error_When_Price_Is_Invalid(decimal data)
        {
            _request.Price = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Have_Error_When_Initial_Stock_Is_Invalid(int data)
        {
            _request.InitialStock = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Pass_When_Request_Is_Valid()
        {
            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }
    }
}
