using WarehouseManagement.ViewModels.Contents.Products;

namespace WarehouseManagement.ViewModels.UnitTest.Contents.Products
{
    public class ProductBaseValidatorTest
    {
        private readonly ProductBaseValidator<ProductBase> _validator;
        private readonly ProductBase _product;

        public ProductBaseValidatorTest()
        {
            _validator = new ProductBaseValidator<ProductBase>();
            _product = new ProductBase
            {
                Name = "Test Product",
                Description = "This is a test product.",
                CategoryId = 1,
                Code = "TP001"
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Have_Error_When_Name_Is_Empty(string name)
        {
            _product.Name = name;

            var result = _validator.Validate(_product);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_CategoryId_Is_Less_Than_Or_Equal_To_Zero()
        {
            _product.CategoryId = 0;

            var result = _validator.Validate(_product);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Pass_Validation_When_All_Properties_Are_Valid()
        {
            var result = _validator.Validate(_product);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            _product.Name = "A";

            var result = _validator.Validate(_product);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Long()
        {
            _product.Name = new string('A', 201);

            var result = _validator.Validate(_product);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Too_Long()
        {
            _product.Description = new string('A', 501);

            var result = _validator.Validate(_product);
            Assert.False(result.IsValid);
        }
    }
}
