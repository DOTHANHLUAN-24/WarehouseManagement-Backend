using WarehouseManagement.ViewModels.Contents.Products;

namespace WarehouseManagement.ViewModels.UnitTest.Contents.Products
{
    public class ProductCommentUpdateRequestValidatorTest
    {
        private ProductCommentUpdateRequestValidator _validator;
        private ProductCommentUpdateRequest _request;

        public ProductCommentUpdateRequestValidatorTest()
        {
            _validator = new ProductCommentUpdateRequestValidator();

            _request = new ProductCommentUpdateRequest
            {
                Content = "This is an updated comment.",
            };
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
