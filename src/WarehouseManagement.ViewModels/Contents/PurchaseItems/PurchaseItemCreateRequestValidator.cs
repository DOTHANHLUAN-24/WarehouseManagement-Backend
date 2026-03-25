using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.PurchaseItems
{
    public class PurchaseItemCreateRequestValidator : AbstractValidator<PurchaseItemCreateRequest>
    {
        public PurchaseItemCreateRequestValidator()
        {
            Include(new PurchaseItemBaseValidator<PurchaseItemCreateRequest>());
        }
    }
}
