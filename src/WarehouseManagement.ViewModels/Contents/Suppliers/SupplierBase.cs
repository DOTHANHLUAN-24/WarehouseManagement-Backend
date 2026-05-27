namespace WarehouseManagement.ViewModels.Contents.Suppliers
{
    public class SupplierBase
    {
        public string SupplierName { get; set; } = string.Empty;
        
        public string? ContactPerson { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        
        public bool IsDeleted { get; set; } = false;
    }
}
