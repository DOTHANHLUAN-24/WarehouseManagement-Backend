using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCommentUpdateRequestValidator : AbstractValidator<ProductCommentUpdateRequest>
    {
        public ProductCommentUpdateRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content in comment is required")
                .MaximumLength(500).WithMessage("Content in comment can not exceed 200 characters");

        }
    }
}
