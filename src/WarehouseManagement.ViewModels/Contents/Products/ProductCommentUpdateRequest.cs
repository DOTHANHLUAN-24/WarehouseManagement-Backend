namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCommentUpdateRequest
    {
        public string Content { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;
    }
}
