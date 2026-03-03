namespace WarehouseManagement.ViewModels.Contents.Warehouses
{
    public class WarehouseBase
    {
        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string Email { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;
    }
}
