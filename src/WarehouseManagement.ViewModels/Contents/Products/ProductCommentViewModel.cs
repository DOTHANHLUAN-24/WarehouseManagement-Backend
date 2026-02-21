namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCommentViewModel
    {
        public int ProductId { get; set; }

        public int? ProductVariantId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int? Rating { get; set; }

        public int? ParentId { get; set; }

        public bool IsApproved { get; set; } = true;
    }
}
