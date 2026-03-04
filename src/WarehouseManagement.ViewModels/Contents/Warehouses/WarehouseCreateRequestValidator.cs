using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Warehouses
{
    public class WarehouseCreateRequestValidator : AbstractValidator<WarehouseCreateRequest>
    {
        public WarehouseCreateRequestValidator()
        {
            Include(new WarehouseBaseValidator<WarehouseCreateRequest>());
        }
    }
}
