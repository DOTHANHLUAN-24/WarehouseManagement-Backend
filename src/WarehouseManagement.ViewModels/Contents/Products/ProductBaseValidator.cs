using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductBaseValidator<T> : AbstractValidator<T>
    where T : ProductBase
    {
        public ProductBaseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MinimumLength(2).WithMessage("Product name must be at least 2 characters")
                .MaximumLength(200).WithMessage("Product name can not exceed 200 characters");

            RuleFor(x => x.Description)
                .Must(d => string.IsNullOrWhiteSpace(d) || d.Trim().Length > 0).WithMessage("Description must not be empty")
                .MaximumLength(500).WithMessage("Description can not exceed 500 characters");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category id must be greater than 0");
        }
    }
}
