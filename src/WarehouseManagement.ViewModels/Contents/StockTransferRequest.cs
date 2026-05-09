using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Contents
{
    public class StockTransferRequest
    {
        [Required]
        public int ProductVariantId { get; set; }

        [Required]
        public int FromWarehouseId { get; set; }

        [Required]
        public int ToWarehouseId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public int? ReferenceId { get; set; }

        public int ReferenceType { get; set; } // cast to Data.Enums.ReferenceType in service

        public string? Note { get; set; }
    }
}