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

            // ========== Identity ==========
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

            // ========== Composite Keys ==========
            builder.Entity<OrderItem>()
                .HasKey(x => new { x.OrderId, x.ProductVariantId });

            builder.Entity<PurchaseItem>()
                .HasKey(x => new { x.PurchaseId, x.ProductVariantId });

            builder.Entity<RolePermission>()
                .HasKey(x => new { x.RoleId, x.PermissionId });

            // ========== Index ==========
            builder.Entity<Order>()
                .HasIndex(x => x.CustomerId);

            builder.Entity<Product>()
                .HasIndex(x => x.CategoryId);

            builder.Entity<ProductVariant>()
                .HasIndex(x => x.ProductId);

            // ========== Permission ==========
            builder.Entity<Permission>()
                .HasIndex(x => new { x.FunctionId, x.Action })
                .IsUnique();

            // ========== Self reference ==========
            builder.Entity<Function>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductComment>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }


        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<CustomerAddress> CustomerAddresses { get; set; } = null!;
        public DbSet<Function> Functions { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<ProductComment> ProductComments { get; set; } = null!;
        public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<PurchaseItem> PurchaseItems { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<Shipment> Shipments { get; set; } = null!;
        public DbSet<StockTransaction> StockTransactions { get; set; } = null!;
        public DbSet<StockSnapshot> StockSnapshots { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Voucher> Vouchers { get; set; } = null!;
        public DbSet<Warehouse> Warehouses { get; set; } = null!;

    }
}
