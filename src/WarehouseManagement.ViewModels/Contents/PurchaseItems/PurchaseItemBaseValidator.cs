using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.PurchaseItems
{
    public class PurchaseItemBaseValidator<T> : AbstractValidator<T>
        where T : PurchaseItemBase
    {
        public PurchaseItemBaseValidator()
        {
            RuleFor(x => x.PurchaseId)
                .GreaterThan(0).WithMessage("Purchase id must be greater than 0");

            RuleFor(x => x.ProductVariantId)
                .GreaterThan(0).WithMessage("Product variant id must be greater than 0");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity is must be greater than 0");

            RuleFor(x => x.UnitCost)
                .GreaterThanOrEqualTo(0).WithMessage("Unit cost is must be greater or equal than 0")
                .PrecisionScale(18, 2, true).WithMessage("Unit cost must be decimal(18, 2)");
        }
    }
}
