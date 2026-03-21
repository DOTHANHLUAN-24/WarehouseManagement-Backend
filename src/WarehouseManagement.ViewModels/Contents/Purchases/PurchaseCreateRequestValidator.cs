using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseCreateRequestValidator :AbstractValidator<PurchaseCreateRequest>
    {
        public PurchaseCreateRequestValidator()
        {
            Include(new PurchaseBaseValidator<PurchaseCreateRequest>());
        }
    }
}
