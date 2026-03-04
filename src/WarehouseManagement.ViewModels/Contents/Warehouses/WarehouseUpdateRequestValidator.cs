using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Warehouses
{
    public class WarehouseUpdateRequestValidator : AbstractValidator<WarehouseUpdateRequest>
    {
        public WarehouseUpdateRequestValidator()
        {
            Include(new WarehouseBaseValidator<WarehouseUpdateRequest>());
        }
    }
}
