using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Categories
{
    public class CategoryCreateRequestValidator : AbstractValidator<CategoryCreateRequest>
    {
        public CategoryCreateRequestValidator()
        {
            Include(new CategoryBaseValidator<CategoryCreateRequest>());
        }
    }
}
