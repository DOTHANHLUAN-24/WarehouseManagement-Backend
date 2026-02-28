using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Suppliers
{
    public class SupplierUpdateRequestValidator : AbstractValidator<SupplierUpdateRequest>
    {
        public SupplierUpdateRequestValidator()
        {
            Include(new SupplierBaseValidator<SupplierUpdateRequest>());
        }
    }
}
