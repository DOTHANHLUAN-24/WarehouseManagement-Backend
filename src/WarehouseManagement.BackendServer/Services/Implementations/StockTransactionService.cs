using WarehouseManagement.BackendServer.Repositories.Interfaces;
using WarehouseManagement.BackendServer.Services.Interfaces;
using WarehouseManagement.ViewModels.Contents;

namespace WarehouseManagement.BackendServer.Services.Implementations
{
    public class StockTransactionService(IStockTransactionRepository repository, ILogger<StockTransactionService> logger) : IStockTransactionService
    {
        public async Task<bool> TransferAsync(StockTransferRequest request)
        {
            if (request.FromWarehouseId == request.ToWarehouseId)
            {
                logger.LogWarning("Transfer request has same source and destination warehouse");
                return false;
            }

            var success = await repository.TransferAsync(
                request.ProductVariantId,
                request.FromWarehouseId,
                request.ToWarehouseId,
                request.Quantity,
                request.Note,
                request.ReferenceId,
                (Data.Enums.ReferenceType)request.ReferenceType
            );

            return success;
        }

        public async Task<IEnumerable<LowStockItemViewModel>> GetLowStockAsync(int threshold)
        {
            var items = await repository.GetLowStockItemsAsync(threshold);
            return items.Select(i => new LowStockItemViewModel
            {
                ProductId = i.ProductId,
                ProductVariantId = i.VariantId,
                VariantName = i.VariantName,
                SKU = i.SKU,
                StockQuantity = i.StockQuantity,
                WarehouseId = i.WarehouseId
            });
        }

        public async Task<int> GetVariantStockAsync(int productVariantId, int? warehouseId = null)
        {
            if (warehouseId.HasValue)
                return await repository.GetVariantStockInWarehouseAsync(productVariantId, warehouseId.Value);
            return await repository.GetVariantStockAsync(productVariantId);
        }
    }
}