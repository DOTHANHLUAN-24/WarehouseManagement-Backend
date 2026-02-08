namespace WarehouseManagement.ViewModels.Systems.Permissions
{
    public class PermissionScreenViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string parentId { get; set; } = string.Empty;

        public bool HasCreate { get; set; }

        public bool HasUpdate { get; set; }

        public bool HasDelete { get; set; }

        public bool HasView { get; set; }

        public bool HasApprove { get; set; }
    }
}
