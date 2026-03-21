using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseBaseValidator<T> : AbstractValidator<T>
        where T : PurchaseBase
    {
        public PurchaseBaseValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Supplier id is required");

            RuleFor(x => x.TotalCost)
                .GreaterThanOrEqualTo(0).WithMessage("Total cost is must be greater or equal than 0")
                .PrecisionScale(18, 2, true).WithMessage("Total cost must be decimal(18, 2)");
        }
    }
}
