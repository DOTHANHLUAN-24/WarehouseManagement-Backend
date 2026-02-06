namespace WarehouseManagement.Domain.Enums
{
    public enum StockTransactionType
    {
        // ===== NHẬP KHO =====
        PurchaseReceipt = 1,     // Nhập từ nhà cung cấp (Purchase)
        CustomerReturn = 2,      // Khách trả hàng
        TransferIn = 3,          // Nhập từ kho khác
        AdjustmentIncrease = 4,  // Điều chỉnh tăng (kiểm kê)

        // ===== XUẤT KHO =====
        SalesIssue = 10,         // Bán hàng
        SupplierReturn = 11,     // Trả hàng cho NCC
        TransferOut = 12,        // Xuất sang kho khác
        AdjustmentDecrease = 13,// Điều chỉnh giảm (kiểm kê)
        Damaged = 14,            // Hư hỏng / hết hạn

        // ===== KHÓA KHO / HỆ THỐNG =====
        InventoryCount = 20,     // Chốt kiểm kê
        SystemCorrection = 21    // Hệ thống tự điều chỉnh
    }
}
