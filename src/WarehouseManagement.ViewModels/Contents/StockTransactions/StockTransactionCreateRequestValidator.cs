using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.StockTransactions
{
    public class StockTransactionCreateRequestValidator : AbstractValidator<StockTransactionCreateRequest>
    {
        public StockTransactionCreateRequestValidator()
        {
            Include(new StockTransactionBaseValidator<StockTransactionCreateRequest>());
        }
    }
}
