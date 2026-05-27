namespace WarehouseManagement.ViewModels.Systems.Customers
{
    public class CustomerViewModel : CustomerBase
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }
    }
}