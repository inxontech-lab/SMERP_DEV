using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.SaasDBModels;

public partial class SmerpContext : DbContext
{
    public SmerpContext()
    {
    }

    public SmerpContext(DbContextOptions<SmerpContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PosTerminal> PosTerminals { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductPrice> ProductPrices { get; set; }

    public virtual DbSet<ProductUom> ProductUoms { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<TaxCode> TaxCodes { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<TenantSetting> TenantSettings { get; set; }

    public virtual DbSet<Uom> Uoms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBranch> UserBranches { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=SMERP;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.CreatedAt }, "IX_AuditLogs_Tenant_CreatedAt").IsDescending(false, true);

            entity.Property(e => e.Action).HasMaxLength(80);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Entity).HasMaxLength(80);
            entity.Property(e => e.EntityId).HasMaxLength(80);
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Code }, "UQ_Branches").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Branches)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Branches_Tenants");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_Permissions_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(80);
            entity.Property(e => e.Module).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<PosTerminal>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.BranchId, e.Code }, "UQ_PosTerminals").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(30);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Branch).WithMany(p => p.PosTerminals)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PosTerminals_Branches");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Barcode }, "IX_Products_Tenant_Barcode").HasFilter("([Barcode] IS NOT NULL)");

            entity.HasIndex(e => new { e.TenantId, e.Name }, "IX_Products_Tenant_Name");

            entity.HasIndex(e => new { e.TenantId, e.Sku }, "UQ_Products_Sku").IsUnique();

            entity.Property(e => e.Barcode).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Sku).HasMaxLength(50);

            entity.HasOne(d => d.BaseUom).WithMany(p => p.Products)
                .HasForeignKey(d => d.BaseUomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Uoms");
        });

        modelBuilder.Entity<ProductPrice>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.ProductId }, "UQ_ProductPrices").IsUnique();

            entity.Property(e => e.SellPrice).HasColumnType("decimal(18, 3)");

            entity.HasOne(d => d.DefaultSellUom).WithMany(p => p.ProductPrices)
                .HasForeignKey(d => d.DefaultSellUomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPrices_Uoms");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPrices)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPrices_Products");

            entity.HasOne(d => d.TaxCode).WithMany(p => p.ProductPrices)
                .HasForeignKey(d => d.TaxCodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPrices_TaxCodes");
        });

        modelBuilder.Entity<ProductUom>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.ProductId }, "IX_ProductUoms_Tenant_Product");

            entity.HasIndex(e => new { e.TenantId, e.ProductId, e.UomId }, "UQ_ProductUoms").IsUnique();

            entity.Property(e => e.FactorToBase).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.IsPurchasable).HasDefaultValue(true);
            entity.Property(e => e.IsSellable).HasDefaultValue(true);
            entity.Property(e => e.MaxDecimals).HasDefaultValue((byte)3);
            entity.Property(e => e.QtyStep)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductUoms)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductUoms_Products");

            entity.HasOne(d => d.Uom).WithMany(p => p.ProductUoms)
                .HasForeignKey(d => d.UomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductUoms_Uoms");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.UserId }, "IX_RefreshTokens_Tenant_User");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedIp).HasMaxLength(45);
            entity.Property(e => e.ExpiresAt).HasPrecision(0);
            entity.Property(e => e.RevokedAt).HasPrecision(0);
            entity.Property(e => e.TokenHash).HasMaxLength(64);
            entity.Property(e => e.UserAgent).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Name }, "UQ_Roles").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Roles)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Roles_Tenants");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.RoleId, e.PermissionId });

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Permissions");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Roles");
        });

        modelBuilder.Entity<TaxCode>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Code }, "UQ_TaxCodes").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Rate).HasColumnType("decimal(9, 4)");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_Tenants_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CountryCode)
                .HasMaxLength(5)
                .HasDefaultValue("BH");
            entity.Property(e => e.CrNo).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LegalName).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.TimeZone)
                .HasMaxLength(60)
                .HasDefaultValue("Asia/Bahrain");
            entity.Property(e => e.VatNo).HasMaxLength(50);
        });

        modelBuilder.Entity<TenantSetting>(entity =>
        {
            entity.HasKey(e => e.TenantId);

            entity.Property(e => e.TenantId).ValueGeneratedNever();
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(10)
                .HasDefaultValue("BHD");
            entity.Property(e => e.InvoicePrefix)
                .HasMaxLength(20)
                .HasDefaultValue("INV");
            entity.Property(e => e.MoneyDecimals).HasDefaultValue((byte)3);
            entity.Property(e => e.PricesVatInclusive).HasDefaultValue(true);
            entity.Property(e => e.QtyDecimals).HasDefaultValue((byte)3);
            entity.Property(e => e.ReceiptPrefix)
                .HasMaxLength(20)
                .HasDefaultValue("POS");
            entity.Property(e => e.VatRateDefault)
                .HasDefaultValue(0.1000m)
                .HasColumnType("decimal(9, 4)");

            entity.HasOne(d => d.Tenant).WithOne(p => p.TenantSetting)
                .HasForeignKey<TenantSetting>(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TenantSettings_Tenants");
        });

        modelBuilder.Entity<Uom>(entity =>
        {
            entity.HasIndex(e => e.Code, "UQ_Uoms_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => new { e.TenantId, e.Username }, "IX_Users_Tenant_Username");

            entity.HasIndex(e => new { e.TenantId, e.Username }, "UQ_Users").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLoginAt).HasPrecision(0);
            entity.Property(e => e.Mobile).HasMaxLength(30);
            entity.Property(e => e.PasswordHash).HasMaxLength(64);
            entity.Property(e => e.PasswordSalt).HasMaxLength(32);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Tenant).WithMany(p => p.Users)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Tenants");
        });

        modelBuilder.Entity<UserBranch>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.UserId, e.BranchId });

            entity.HasOne(d => d.Branch).WithMany(p => p.UserBranches)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserBranches_Branches");

            entity.HasOne(d => d.User).WithMany(p => p.UserBranches)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserBranches_Users");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.UserId, e.RoleId });

            entity.HasIndex(e => e.UserId, "IX_UserRoles_UserId");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
