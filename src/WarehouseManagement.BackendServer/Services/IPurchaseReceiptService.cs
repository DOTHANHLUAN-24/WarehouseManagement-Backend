using WarehouseManagement.ViewModels.Contents.PurchaseReceipts;

namespace WarehouseManagement.BackendServer.Services
{
    public interface IPurchaseReceiptService
    {
        Task<PurchaseReceiptResponse> CreateAsync(PurchaseReceiptRequest request);
        Task<PurchaseReceiptResponse?> GetByIdAsync(int id);
        Task<List<PurchaseReceiptResponse>> GetAllAsync();
        Task<bool> UpdateAsync(int id, PurchaseReceiptRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
