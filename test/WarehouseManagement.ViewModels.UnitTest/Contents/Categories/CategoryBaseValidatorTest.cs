using WarehouseManagement.ViewModels.Contents.Categories;

namespace WarehouseManagement.ViewModels.UnitTest.Contents.Categories
{
    public class CategoryBaseValidatorTest
    {
        private readonly CategoryBaseValidator<CategoryBase> _validator;
        private readonly CategoryBase _request;

        public CategoryBaseValidatorTest()
        {
            _validator = new CategoryBaseValidator<CategoryBase>();
            _request = new CategoryBase
            {
                Name = "Category 1",
                SeoAlias = "category-1",
                SeoDescription = "This is category 1",
                SortOrder = 1,
                ParentId = null
            };
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Name(string data)
        {
            _request.Name = data;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Seo_Alias(string data)
        {
            _request.SeoAlias = data;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Return_Error_When_Missing_Seo_Description(string data)
        {
            _request.SeoDescription = data;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Return_Error_When_Invalid_Sort_Order(int sortOrder)
        {
            _request.SortOrder = sortOrder;
            var result = _validator.Validate(_request);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("category name", "seo-alias", "al;sdkl;kdal;", 1, null)]
        [InlineData("category name", "seo-alias", "al;sdkl;kdal;", 1, 1)]
        public void Should_Return_Success_When_Valid_Request(string name, string seoAlias, string seoDescription, int sortOrder, int? parentId)
        {
            _request.Name = name;
            _request.SeoAlias = seoAlias;
            _request.SeoDescription = seoDescription;
            _request.SortOrder = sortOrder;
            _request.ParentId = parentId;

            var result = _validator.Validate(_request);
            Assert.True(result.IsValid);
        }
    }
}
