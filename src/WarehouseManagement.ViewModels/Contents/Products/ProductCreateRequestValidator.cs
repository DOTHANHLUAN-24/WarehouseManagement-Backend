using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
    {
        public ProductCreateRequestValidator()
        {
            Include(new ProductBaseValidator<ProductCreateRequest>());
        }
    }
}
