using FluentValidation;

namespace WarehouseManagement.ViewModels.Contents.StockTransactions
{
    public class StockTransactionUpdateRequestValidator : AbstractValidator<StockTransactionUpdateRequest>
    {
        public StockTransactionUpdateRequestValidator()
        {
            Include(new StockTransactionBaseValidator<StockTransactionUpdateRequest>());
        }
    }
}
