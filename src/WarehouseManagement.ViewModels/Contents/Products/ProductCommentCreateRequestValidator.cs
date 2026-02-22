using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCommentCreateRequestValidator : AbstractValidator<ProductCommentCreateRequest>
    {
        public ProductCommentCreateRequestValidator() {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product id in comment is required");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id in comment is required");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content in comment is required")
                .MaximumLength(500).WithMessage("Content in comment can not exceed 500 characters");

        }
    }
}
