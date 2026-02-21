using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductVariantViewModel : ProductBase
    {
        public int ProductVariantId  { get; set; }

        public string? SKU { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public bool IsActiveInVariant { get; set; } = true;

    }
}
