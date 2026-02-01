using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Enums;
using WarehouseManagement.BackendServer.Data.Interfaces;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Vouchers")]
    public class Voucher : IDateTracking
    {
        [Key]
        public int Id { get; set; }

        public string? Code { get; set; }

        [Required]
        public VoucherApplyType ApplyType { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Precision(18, 2)]
        public decimal DiscountValue { get; set; }

        [Precision(18, 2)]
        public decimal? MaxDiscountAmount { get; set; }

        [Precision(18, 2)]
        public decimal? MinOrderAmount { get; set; }

        public bool OnlyForNewCustomer { get; set; }  

        public int Priority { get; set; }            
        
        public bool IsStackable { get; set; }    
        
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; }

        public int UsedCount { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
