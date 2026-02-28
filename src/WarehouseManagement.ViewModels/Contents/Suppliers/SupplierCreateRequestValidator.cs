using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Suppliers
{
    public class SupplierCreateRequestValidator : AbstractValidator<SupplierCreateRequest>
    {
        public SupplierCreateRequestValidator()
        {
            Include(new SupplierBaseValidator<SupplierCreateRequest>());
        }
    }
}
