using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.StockTransactions
{
    public class StockTransactionBaseValidator<T> : AbstractValidator<T>
        where T : StockTransactionBase
    {
        public StockTransactionBaseValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product id must be greater than 0");

            RuleFor(x => x.WarehouseId)
                .GreaterThan(0).WithMessage("Warehouse id must be greater than 0");

            RuleFor(x => x.QuantityChange)
                .GreaterThan(0).WithMessage("Quantity change must be greater than 0");

            RuleFor(x => x.BalanceAfter)
                .GreaterThan(0).WithMessage("Balance after must be greater than 0");
        }
    }
}
