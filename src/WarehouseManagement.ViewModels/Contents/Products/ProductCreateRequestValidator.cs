using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
    {
        public ProductCreateRequestValidator()
        {
            Include(new ProductBaseValidator<ProductCreateRequest>());

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price in product is required");

            RuleFor(x => x.InitialStock)
                .NotEmpty().WithMessage("Initial Stock");
        }
    }
}
