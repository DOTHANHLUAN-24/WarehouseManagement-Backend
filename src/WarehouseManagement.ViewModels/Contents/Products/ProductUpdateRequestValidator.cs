using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
    {
        public ProductUpdateRequestValidator() 
        {
            Include(new ProductBaseValidator<ProductUpdateRequest>());
        }
    }
}
