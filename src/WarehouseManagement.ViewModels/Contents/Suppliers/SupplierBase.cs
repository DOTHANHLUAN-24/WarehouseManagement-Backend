namespace WarehouseManagement.ViewModels.Contents.Suppliers
{
    public class SupplierBase
    {
        public string SupplierName { get; set; } = string.Empty;
        
        public string ContactPerson { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public bool IsDeleted { get; set; } = false;
    }
}
