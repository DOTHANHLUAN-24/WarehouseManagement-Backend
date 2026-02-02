namespace WarehouseManagement.ViewModels.Systems
{
    public class Pagination<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
    }
}
