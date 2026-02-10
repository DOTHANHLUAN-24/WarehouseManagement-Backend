using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Payments")]
    public class Payment : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public string Method { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
