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

    public virtual DbSet<AuditLog1> AuditLogs1 { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<InvAdjustmentHeader> InvAdjustmentHeaders { get; set; }

    public virtual DbSet<InvAdjustmentLine> InvAdjustmentLines { get; set; }

    public virtual DbSet<InvGrnheader> InvGrnheaders { get; set; }

    public virtual DbSet<InvGrnline> InvGrnlines { get; set; }

    public virtual DbSet<InvItem> InvItems { get; set; }

    public virtual DbSet<InvItemCategory> InvItemCategories { get; set; }

    public virtual DbSet<InvItemSubCategory> InvItemSubCategories { get; set; }

    public virtual DbSet<InvItemUomconversion> InvItemUomconversions { get; set; }

    public virtual DbSet<InvStockBalance> InvStockBalances { get; set; }

    public virtual DbSet<InvStockTxn> InvStockTxns { get; set; }

    public virtual DbSet<InvSupplier> InvSuppliers { get; set; }

    public virtual DbSet<InvTransferHeader> InvTransferHeaders { get; set; }

    public virtual DbSet<InvTransferLine> InvTransferLines { get; set; }

    public virtual DbSet<InvUom> InvUoms { get; set; }

    public virtual DbSet<InvWarehouse> InvWarehouses { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PosTerminal> PosTerminals { get; set; }

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
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

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

        modelBuilder.Entity<AuditLog1>(entity =>
        {
            entity.ToTable("AuditLog", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.TableName, e.RecordId, e.ChangedAt }, "IX_AuditLog_Tenant_Table_Record").IsDescending(false, false, false, true);

            entity.Property(e => e.ActionType).HasMaxLength(10);
            entity.Property(e => e.AppName).HasMaxLength(128);
            entity.Property(e => e.ChangedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ChangedBy).HasMaxLength(100);
            entity.Property(e => e.HostName).HasMaxLength(128);
            entity.Property(e => e.RecordId).HasMaxLength(100);
            entity.Property(e => e.TableName).HasMaxLength(128);
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

        modelBuilder.Entity<InvAdjustmentHeader>(entity =>
        {
            entity.ToTable("InvAdjustmentHeader", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.BranchId, e.AdjustNo }, "UQ_InvAdjustmentHeader_Tenant_Branch_AdjustNo").IsUnique();

            entity.Property(e => e.AdjustNo).HasMaxLength(40);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.PostedAt).HasPrecision(0);
            entity.Property(e => e.PostedBy).HasMaxLength(100);
            entity.Property(e => e.Reason).HasMaxLength(300);
            entity.Property(e => e.ReasonAr).HasMaxLength(300);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("DRAFT");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvAdjustmentHeaders)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvAdjustmentHeader_Branches");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvAdjustmentHeaders)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvAdjustmentHeader_Tenants");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InvAdjustmentHeaders)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvAdjustmentHeader_Warehouse");
        });

        modelBuilder.Entity<InvAdjustmentLine>(entity =>
        {
            entity.ToTable("InvAdjustmentLine", "inventory");

            entity.HasIndex(e => new { e.AdjustmentHeaderId, e.LineNo }, "UQ_InvAdjustmentLine_Header_LineNo").IsUnique();

            entity.Property(e => e.BatchNo).HasMaxLength(80);
            entity.Property(e => e.QtyDelta).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Reason).HasMaxLength(300);
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Uomid).HasColumnName("UOMId");

            entity.HasOne(d => d.AdjustmentHeader).WithMany(p => p.InvAdjustmentLines)
                .HasForeignKey(d => d.AdjustmentHeaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvAdjustmentLine_Header");

            entity.HasOne(d => d.Item).WithMany(p => p.InvAdjustmentLines)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvAdjustmentLine_Item");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvAdjustmentLines)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvAdjustmentLine_Tenants");

            entity.HasOne(d => d.Uom).WithMany(p => p.InvAdjustmentLines)
                .HasForeignKey(d => d.Uomid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvAdjustmentLine_UOM");
        });

        modelBuilder.Entity<InvGrnheader>(entity =>
        {
            entity.ToTable("InvGRNHeader", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.BranchId, e.Grnno }, "UQ_InvGRNHeader_Tenant_Branch_GRNNo").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CurrencyCode).HasMaxLength(10);
            entity.Property(e => e.ExchangeRate)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Grndate).HasColumnName("GRNDate");
            entity.Property(e => e.Grnno)
                .HasMaxLength(40)
                .HasColumnName("GRNNo");
            entity.Property(e => e.ModifiedAt).HasPrecision(0);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("DRAFT");
            entity.Property(e => e.SupplierInvoiceNo).HasMaxLength(60);

            entity.HasOne(d => d.Branch).WithMany(p => p.InvGrnheaders)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvGRNHeader_Branches");

            entity.HasOne(d => d.Supplier).WithMany(p => p.InvGrnheaders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvGRNHeader_Supplier");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvGrnheaders)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvGRNHeader_Tenants");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InvGrnheaders)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvGRNHeader_Warehouse");
        });

        modelBuilder.Entity<InvGrnline>(entity =>
        {
            entity.ToTable("InvGRNLine", "inventory");

            entity.HasIndex(e => new { e.GrnheaderId, e.LineNo }, "UQ_InvGRNLine_Header_LineNo").IsUnique();

            entity.Property(e => e.BatchNo).HasMaxLength(80);
            entity.Property(e => e.GrnheaderId).HasColumnName("GRNHeaderId");
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Remarks).HasMaxLength(300);
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Uomid).HasColumnName("UOMId");
            entity.Property(e => e.Vatpercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("VATPercent");

            entity.HasOne(d => d.Grnheader).WithMany(p => p.InvGrnlines)
                .HasForeignKey(d => d.GrnheaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvGRNLine_Header");

            entity.HasOne(d => d.Item).WithMany(p => p.InvGrnlines)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvGRNLine_Item");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvGrnlines)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvGRNLine_Tenants");

            entity.HasOne(d => d.Uom).WithMany(p => p.InvGrnlines)
                .HasForeignKey(d => d.Uomid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvGRNLine_UOM");
        });

        modelBuilder.Entity<InvItem>(entity =>
        {
            entity.ToTable("InvItem", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.Barcode }, "UQ_InvItem_Tenant_Barcode").IsUnique();

            entity.HasIndex(e => new { e.TenantId, e.Code }, "UQ_InvItem_Tenant_Code").IsUnique();

            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.BaseUomid).HasColumnName("BaseUOMId");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CountryOfOrigin).HasMaxLength(10);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Hscode)
                .HasMaxLength(30)
                .HasColumnName("HSCode");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxStockLevel).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.MinStockLevel).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ModifiedAt).HasPrecision(0);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.NameAr).HasMaxLength(250);
            entity.Property(e => e.PurchaseUomid).HasColumnName("PurchaseUOMId");
            entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.SalesUomid).HasColumnName("SalesUOMId");
            entity.Property(e => e.SellingPrice).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.StandardCost).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Vatpercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("VATPercent");

            entity.HasOne(d => d.BaseUom).WithMany(p => p.InvItemBaseUoms)
                .HasForeignKey(d => d.BaseUomid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItem_BaseUOM");

            entity.HasOne(d => d.Category).WithMany(p => p.InvItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItem_Category");

            entity.HasOne(d => d.PurchaseUom).WithMany(p => p.InvItemPurchaseUoms)
                .HasForeignKey(d => d.PurchaseUomid)
                .HasConstraintName("FK_InvItem_PurchaseUOM");

            entity.HasOne(d => d.SalesUom).WithMany(p => p.InvItemSalesUoms)
                .HasForeignKey(d => d.SalesUomid)
                .HasConstraintName("FK_InvItem_SalesUOM");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.InvItems)
                .HasForeignKey(d => d.SubCategoryId)
                .HasConstraintName("FK_InvItem_SubCategory");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvItems)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItem_Tenants");
        });

        modelBuilder.Entity<InvItemCategory>(entity =>
        {
            entity.ToTable("InvItemCategory", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.Code }, "UQ_InvItemCategory_Tenant_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(30);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasPrecision(0);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.NameAr).HasMaxLength(200);

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvItemCategories)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItemCategory_Tenants");
        });

        modelBuilder.Entity<InvItemSubCategory>(entity =>
        {
            entity.ToTable("InvItemSubCategory", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.CategoryId, e.Code }, "UQ_InvItemSubCategory_Tenant_Category_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(30);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasPrecision(0);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.NameAr).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.InvItemSubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItemSubCategory_Category");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvItemSubCategories)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItemSubCategory_Tenants");
        });

        modelBuilder.Entity<InvItemUomconversion>(entity =>
        {
            entity.ToTable("InvItemUOMConversion", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.ItemId, e.FromUomid, e.ToUomid }, "UQ_InvItemUOMConversion").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Factor).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.FromUomid).HasColumnName("FromUOMId");
            entity.Property(e => e.ToUomid).HasColumnName("ToUOMId");

            entity.HasOne(d => d.FromUom).WithMany(p => p.InvItemUomconversionFromUoms)
                .HasForeignKey(d => d.FromUomid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItemUOMConversion_FromUOM");

            entity.HasOne(d => d.Item).WithMany(p => p.InvItemUomconversions)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItemUOMConversion_Item");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvItemUomconversions)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItemUOMConversion_Tenants");

            entity.HasOne(d => d.ToUom).WithMany(p => p.InvItemUomconversionToUoms)
                .HasForeignKey(d => d.ToUomid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvItemUOMConversion_ToUOM");
        });

        modelBuilder.Entity<InvStockBalance>(entity =>
        {
            entity.ToTable("InvStockBalance", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.BranchId, e.WarehouseId, e.ItemId, e.BatchNo, e.ExpiryDate }, "UX_InvStockBalance_Key").IsUnique();

            entity.Property(e => e.AvgCost).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.BatchNo).HasMaxLength(80);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.LastTxnAt).HasPrecision(0);
            entity.Property(e => e.ModifiedAt).HasPrecision(0);
            entity.Property(e => e.QtyOnHand).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.QtyReserved).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvStockBalances)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvStockBalance_Branches");

            entity.HasOne(d => d.Item).WithMany(p => p.InvStockBalances)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvStockBalance_Item");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvStockBalances)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvStockBalance_Tenants");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InvStockBalances)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvStockBalance_Warehouse");
        });

        modelBuilder.Entity<InvStockTxn>(entity =>
        {
            entity.ToTable("InvStockTxn", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.ItemId, e.TxnDate }, "IX_InvStockTxn_Tenant_Item_Date");

            entity.Property(e => e.Amount)
                .HasComputedColumnSql("(([QtyIn]-[QtyOut])*[UnitCost])", true)
                .HasColumnType("decimal(38, 12)");
            entity.Property(e => e.BatchNo).HasMaxLength(80);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.QtyIn).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.QtyOut).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.RefNo).HasMaxLength(50);
            entity.Property(e => e.RefType).HasMaxLength(30);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.TxnDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TxnType).HasMaxLength(30);
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvStockTxns)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvStockTxn_Branches");

            entity.HasOne(d => d.Item).WithMany(p => p.InvStockTxns)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvStockTxn_Item");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvStockTxns)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvStockTxn_Tenants");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InvStockTxns)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvStockTxn_Warehouse");
        });

        modelBuilder.Entity<InvSupplier>(entity =>
        {
            entity.ToTable("InvSupplier", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.Code }, "UQ_InvSupplier_Tenant_Code").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.AddressAr).HasMaxLength(300);
            entity.Property(e => e.Code).HasMaxLength(30);
            entity.Property(e => e.ContactPerson).HasMaxLength(150);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Crno)
                .HasMaxLength(50)
                .HasColumnName("CRNo");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasPrecision(0);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.NameAr).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.VatregistrationNo)
                .HasMaxLength(50)
                .HasColumnName("VATRegistrationNo");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvSuppliers)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvSupplier_Tenants");
        });

        modelBuilder.Entity<InvTransferHeader>(entity =>
        {
            entity.ToTable("InvTransferHeader", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.TransferNo }, "UQ_InvTransferHeader_Tenant_TransferNo").IsUnique();

            entity.Property(e => e.ApprovedAt).HasPrecision(0);
            entity.Property(e => e.ApprovedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ReceivedAt).HasPrecision(0);
            entity.Property(e => e.ReceivedBy).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("DRAFT");
            entity.Property(e => e.TransferNo).HasMaxLength(40);

            entity.HasOne(d => d.FromBranch).WithMany(p => p.InvTransferHeaderFromBranches)
                .HasForeignKey(d => d.FromBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferHeader_FromBranch");

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.InvTransferHeaderFromWarehouses)
                .HasForeignKey(d => d.FromWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferHeader_FromWarehouse");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvTransferHeaders)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferHeader_Tenants");

            entity.HasOne(d => d.ToBranch).WithMany(p => p.InvTransferHeaderToBranches)
                .HasForeignKey(d => d.ToBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferHeader_ToBranch");

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.InvTransferHeaderToWarehouses)
                .HasForeignKey(d => d.ToWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferHeader_ToWarehouse");
        });

        modelBuilder.Entity<InvTransferLine>(entity =>
        {
            entity.ToTable("InvTransferLine", "inventory");

            entity.HasIndex(e => new { e.TransferHeaderId, e.LineNo }, "UQ_InvTransferLine_Header_LineNo").IsUnique();

            entity.Property(e => e.BatchNo).HasMaxLength(80);
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Remarks).HasMaxLength(300);
            entity.Property(e => e.Uomid).HasColumnName("UOMId");

            entity.HasOne(d => d.Item).WithMany(p => p.InvTransferLines)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferLine_Item");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvTransferLines)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferLine_Tenants");

            entity.HasOne(d => d.TransferHeader).WithMany(p => p.InvTransferLines)
                .HasForeignKey(d => d.TransferHeaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferLine_Header");

            entity.HasOne(d => d.Uom).WithMany(p => p.InvTransferLines)
                .HasForeignKey(d => d.Uomid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvTransferLine_UOM");
        });

        modelBuilder.Entity<InvUom>(entity =>
        {
            entity.ToTable("InvUOM", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.Code }, "UQ_InvUOM_Tenant_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasPrecision(0);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NameAr).HasMaxLength(100);

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvUoms)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvUOM_Tenants");
        });

        modelBuilder.Entity<InvWarehouse>(entity =>
        {
            entity.ToTable("InvWarehouse", "inventory");

            entity.HasIndex(e => new { e.TenantId, e.BranchId, e.Code }, "UQ_InvWarehouse_Tenant_Branch_Code").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.Code).HasMaxLength(30);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasPrecision(0);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.NameAr).HasMaxLength(200);

            entity.HasOne(d => d.Branch).WithMany(p => p.InvWarehouses)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvWarehouse_Branches");

            entity.HasOne(d => d.Tenant).WithMany(p => p.InvWarehouses)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvWarehouse_Tenants");
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
