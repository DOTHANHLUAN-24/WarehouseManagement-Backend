using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.PurchaseItems
{
    public class PurchaseItemUpdateRequestValidator : AbstractValidator<PurchaseItemUpdateRequest>
    {
        public PurchaseItemUpdateRequestValidator()
        {
            Include(new PurchaseItemBaseValidator<PurchaseItemUpdateRequest>());
        }
    }
}
