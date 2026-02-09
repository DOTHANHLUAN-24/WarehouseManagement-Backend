using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Categories
{
    public class CategoryUpdateRequestValidator : AbstractValidator<CategoryUpdateRequest>
    {
        public CategoryUpdateRequestValidator()
        {
            Include(new CategoryBaseValidator<CategoryUpdateRequest>());
        }
    }
}
