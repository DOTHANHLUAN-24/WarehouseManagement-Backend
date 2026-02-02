using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("StockTransactions")]
    public class StockTransaction : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required]
        public int QuantityChange { get; set; }

        [Required]

        public StockTransactionType TransactionType { get; set; }

        [Required]
        public string ReferenceType { get; set; } = string.Empty; // Order / Purchase

        [Required]
        public int ReferenceId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
