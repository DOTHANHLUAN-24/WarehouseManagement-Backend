using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Domain.Entities
{
    public class Payment : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public string Method { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
