namespace WarehouseManagement.BackendServer.Repositories.Interfaces
{
    public interface IStockTransactionRepository
    {
        Task<bool> TransferAsync(int productVariantId, int fromWarehouseId, int toWarehouseId, int quantity, string? note, int? referenceId, Data.Enums.ReferenceType referenceType);
        Task<int> GetVariantStockAsync(int productVariantId);
        Task<int> GetVariantStockInWarehouseAsync(int productVariantId, int warehouseId);
        Task<IEnumerable<(int ProductId, int VariantId, string? VariantName, string? SKU, int StockQuantity, int WarehouseId)>> GetLowStockItemsAsync(int threshold);
    }
}