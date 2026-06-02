using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
    {
        public ProductCreateRequestValidator()
        {
            Include(new ProductBaseValidator<ProductCreateRequest>());

            RuleFor(x => x.SellingPrice)
                .NotNull().WithMessage("Selling price in product is required")
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.InitialStock)
                .NotNull().WithMessage("Initial Stock in product is required")
                .GreaterThanOrEqualTo(0).WithMessage("Initial Stock must be greater than or equal to 0.");
        }
    }
}
