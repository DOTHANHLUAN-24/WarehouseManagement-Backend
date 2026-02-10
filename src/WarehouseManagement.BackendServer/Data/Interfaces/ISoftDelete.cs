namespace WarehouseManagement.BackendServer.Data.Interfaces
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
