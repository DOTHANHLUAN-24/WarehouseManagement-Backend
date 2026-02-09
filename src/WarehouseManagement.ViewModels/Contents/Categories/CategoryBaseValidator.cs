using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Categories
{
    public class CategoryBaseValidator<T> : AbstractValidator<T>
        where T : CategoryBase
    {
        public CategoryBaseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(200) .WithMessage("Category name can not exceed 200 characters");

            RuleFor(x => x.SeoAlias)
                .NotEmpty().WithMessage("SEO alias is required")
                .MaximumLength(200).WithMessage("SEO alias can not exceed 200 characters");

            RuleFor(x => x.SeoDescription)
                .NotEmpty().WithMessage("SEO description is required")
                .MaximumLength(500).WithMessage("SEO description can not exceed 500 characters");

            RuleFor(x => x.SortOrder)
                .GreaterThan(0).WithMessage("Sort order is no valid!");
        }
    }
}
