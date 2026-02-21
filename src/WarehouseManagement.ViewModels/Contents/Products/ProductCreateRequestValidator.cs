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
                .GreaterThan(0).WithMessage("Initial Stock must be greater than 0.");
        }
    }
}
