using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations
{
    public class IdentityConfiguration :
        IEntityTypeConfiguration<User>,
        IEntityTypeConfiguration<IdentityRole>,
        IEntityTypeConfiguration<IdentityUserRole<string>>,
        IEntityTypeConfiguration<IdentityUserClaim<string>>,
        IEntityTypeConfiguration<IdentityRoleClaim<string>>,
        IEntityTypeConfiguration<IdentityUserLogin<string>>,
        IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id)
                   .HasMaxLength(50)
                   .IsUnicode(false);
        }

        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.Property(x => x.Id)
                   .HasMaxLength(50)
                   .IsUnicode(false);
        }

        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.Property(x => x.UserId).HasMaxLength(50).IsUnicode(false);
            builder.Property(x => x.RoleId).HasMaxLength(50).IsUnicode(false);
        }

        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.Property(x => x.UserId)
                   .HasMaxLength(50)
                   .IsUnicode(false);
        }

        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.Property(x => x.RoleId)
                   .HasMaxLength(50)
                   .IsUnicode(false);
        }

        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder.Property(x => x.UserId)
                   .HasMaxLength(50)
                   .IsUnicode(false);
        }

        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder.Property(x => x.UserId)
                   .HasMaxLength(50)
                   .IsUnicode(false);
        }
    }
}
