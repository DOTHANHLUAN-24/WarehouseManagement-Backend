using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseUpdateRequestValidator : AbstractValidator<PurchaseUpdateRequest>
    {
        public PurchaseUpdateRequestValidator()
        {
            Include(new PurchaseBaseValidator<PurchaseUpdateRequest>());
        }
    }
}
