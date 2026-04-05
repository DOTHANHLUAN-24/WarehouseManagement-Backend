namespace WarehouseManagement.ViewModels.Contents.Categories
{
    public class CategoryBase
    {
        public string Name { get; set; } = string.Empty;

        public string SeoAlias { get; set; } = string.Empty;

        public string SeoDescription { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public int? ParentId { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
