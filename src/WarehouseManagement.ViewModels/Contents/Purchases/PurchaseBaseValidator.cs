using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.Purchases
{
    public class PurchaseBaseValidator<T> : AbstractValidator<T>
        where T : PurchaseBase
    {
        public PurchaseBaseValidator()
        {
            RuleFor(x => x.Type)
                .Must(x => x == 1 || x == 2).WithMessage("Type must be 1 (Import) or 2 (Export)");

            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Supplier id is required")
                .When(x => x.Type == 1);

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer id is required")
                .When(x => x.Type == 2);

            RuleFor(x => x.TotalCost)
                .GreaterThanOrEqualTo(0).WithMessage("Total cost is must be greater or equal than 0")
                .PrecisionScale(18, 2, true).WithMessage("Total cost must be decimal(18, 2)");
        }
    }
}
