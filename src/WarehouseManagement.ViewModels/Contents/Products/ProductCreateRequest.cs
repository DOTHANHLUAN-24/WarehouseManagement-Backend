using Microsoft.AspNetCore.Http;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCreateRequest : ProductBase
    {
        public decimal SellingPrice { get; set; }

        public decimal OriginalPrice { get; set; }

        public int InitialStock { get; set; }
        
        public string? SKU { get; set; } // IMEI
    }
}
