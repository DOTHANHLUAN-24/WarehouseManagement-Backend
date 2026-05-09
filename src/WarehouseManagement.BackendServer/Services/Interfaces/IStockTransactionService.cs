using WarehouseManagement.ViewModels.Contents;

namespace WarehouseManagement.BackendServer.Services.Interfaces
{
    public interface IStockTransactionService
    {
        Task<bool> TransferAsync(StockTransferRequest request);
        Task<IEnumerable<LowStockItemViewModel>> GetLowStockAsync(int threshold);
        Task<int> GetVariantStockAsync(int productVariantId, int? warehouseId = null);
    }
}