using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Domain.Entities
{
    public class Shipment : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        [Required]
        public int WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; } = null!;

        public DateTime ShipmentDate { get; set; }

        public ShipmentStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
