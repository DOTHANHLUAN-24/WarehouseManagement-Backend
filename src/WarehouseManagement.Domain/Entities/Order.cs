using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Domain.Entities
{
    public class Order : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public Customer Customer { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Shipment? Shipment { get; set; }

        public Payment? Payment { get; set; }

        public int? VoucherId { get; set; }

        public Voucher? Voucher { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
