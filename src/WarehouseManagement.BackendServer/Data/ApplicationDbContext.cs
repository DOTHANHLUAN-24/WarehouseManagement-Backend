using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.BackendServer.Data.Entities;

namespace WarehouseManagement.BackendServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity
            builder.Entity<IdentityRole>()
                .Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<User>()
                .Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<IdentityUserRole<string>>()
                .Property(x => x.UserId).HasMaxLength(50).IsUnicode(false);
            builder.Entity<IdentityUserRole<string>>()
                .Property(x => x.RoleId).HasMaxLength(50).IsUnicode(false);
            builder.Entity<IdentityUserClaim<string>>()
                .Property(x => x.UserId).HasMaxLength(50).IsUnicode(false);
            builder.Entity<IdentityRoleClaim<string>>()
                .Property(x => x.RoleId).HasMaxLength(50).IsUnicode(false);
            builder.Entity<IdentityUserLogin<string>>()
                .Property(x => x.UserId).HasMaxLength(50).IsUnicode(false);
            builder.Entity<IdentityUserToken<string>>()
                .Property(x => x.UserId).HasMaxLength(50).IsUnicode(false);

            // Rule for basic table in db 
            builder.Entity<User>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<OrderItem>().HasKey(oi => new { oi.OrderId, oi.ProductId });
            builder.Entity<PurchaseItem>().HasKey(pi => new { pi.PurchaseId, pi.ProductId });
            builder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Unique permission
            builder.Entity<Permission>()
                .HasIndex(p => new { p.FunctionId, p.Action })
                .IsUnique();

            // Reference in function
            builder.Entity<Function>()
                .HasOne(f => f.Parent)
                .WithMany(f => f.Children)
                .HasForeignKey(f => f.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Function> Functions { get; set; }

    }
}
