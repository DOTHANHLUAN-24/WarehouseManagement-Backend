using WarehouseManagement.ViewModels.Contents.Products;

namespace WarehouseManagement.ViewModels.UnitTest.Contents.Products
{
    public class ProductCommentCreateRequestValidatorTest
    {
        private ProductCommentCreateRequestValidator _validator;
        private ProductCommentCreateRequest _request;

        public ProductCommentCreateRequestValidatorTest()
        {
            _validator = new ProductCommentCreateRequestValidator();
            _request = new ProductCommentCreateRequest
            {
                ProductId = 1,
                ProductVariantId = 1,
                UserId = "user123",
                Content = "This is a comment.",
                Rating = 5,
                ParentId = null
            };
        }

        [Fact]
        public void Should_Have_Error_When_Missing_ProductId()
        {
            _request.ProductId = 0;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Missing_UserId(string data)
        {
            _request.UserId = data;

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Missing_Content(string data)
        {
            _request.Content = data;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_Content_Exceeds_Max_Length()
        {
            _request.Content = new string('a', 501);

            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Pass_Validation_When_Request_Is_Valid()
        {
            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }
    }
}
