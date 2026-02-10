using Microsoft.AspNetCore.Http;

namespace WarehouseManagement.ViewModels.Contents.Products
{
    public class ProductCreateRequest : ProductBase
    {
        public decimal Price { get; set; }

        public int InitialStock { get; set; }
        
        public string? SKU { get; set; } // IMEI

        public IFormFile? ImageFile { get; set; }
    }
}
