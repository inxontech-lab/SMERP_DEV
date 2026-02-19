/* GCC-focused product catalog upgrade script for retail + wholesale */

SET XACT_ABORT ON;
BEGIN TRANSACTION;

/* -------------------------------
   PRODUCTS: Arabic + VAT at product level + inventory flags
-------------------------------- */
IF COL_LENGTH('dbo.Products', 'NameArabic') IS NULL
    ALTER TABLE dbo.Products ADD NameArabic NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.Products', 'ShortName') IS NULL
    ALTER TABLE dbo.Products ADD ShortName NVARCHAR(100) NULL;

IF COL_LENGTH('dbo.Products', 'Description') IS NULL
    ALTER TABLE dbo.Products ADD [Description] NVARCHAR(1000) NULL;

IF COL_LENGTH('dbo.Products', 'DescriptionArabic') IS NULL
    ALTER TABLE dbo.Products ADD DescriptionArabic NVARCHAR(1000) NULL;

IF COL_LENGTH('dbo.Products', 'TaxCodeId') IS NULL
    ALTER TABLE dbo.Products ADD TaxCodeId INT NULL;

IF COL_LENGTH('dbo.Products', 'IsVatApplicable') IS NULL
    ALTER TABLE dbo.Products ADD IsVatApplicable BIT NOT NULL CONSTRAINT DF_Products_IsVatApplicable DEFAULT (1);

IF COL_LENGTH('dbo.Products', 'VatPricingMethod') IS NULL
    ALTER TABLE dbo.Products ADD VatPricingMethod TINYINT NOT NULL CONSTRAINT DF_Products_VatPricingMethod DEFAULT (1); -- 1=Exclusive, 2=Inclusive

IF COL_LENGTH('dbo.Products', 'IsBatchTracked') IS NULL
    ALTER TABLE dbo.Products ADD IsBatchTracked BIT NOT NULL CONSTRAINT DF_Products_IsBatchTracked DEFAULT (0);

IF COL_LENGTH('dbo.Products', 'IsSerialTracked') IS NULL
    ALTER TABLE dbo.Products ADD IsSerialTracked BIT NOT NULL CONSTRAINT DF_Products_IsSerialTracked DEFAULT (0);

IF COL_LENGTH('dbo.Products', 'IsStockItem') IS NULL
    ALTER TABLE dbo.Products ADD IsStockItem BIT NOT NULL CONSTRAINT DF_Products_IsStockItem DEFAULT (1);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Products_TaxCodes')
BEGIN
    ALTER TABLE dbo.Products WITH NOCHECK
        ADD CONSTRAINT FK_Products_TaxCodes FOREIGN KEY (TaxCodeId) REFERENCES dbo.TaxCodes (Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Tenant_NameArabic' AND object_id = OBJECT_ID('dbo.Products'))
    CREATE INDEX IX_Products_Tenant_NameArabic ON dbo.Products (TenantId, NameArabic) WHERE NameArabic IS NOT NULL;

/* -------------------------------
   PRODUCT PRICES: support retail/wholesale/multi-UOM/branch price
-------------------------------- */
IF COL_LENGTH('dbo.ProductPrices', 'UomId') IS NULL
    ALTER TABLE dbo.ProductPrices ADD UomId INT NULL;

IF COL_LENGTH('dbo.ProductPrices', 'BranchId') IS NULL
    ALTER TABLE dbo.ProductPrices ADD BranchId INT NULL;

IF COL_LENGTH('dbo.ProductPrices', 'PriceType') IS NULL
    ALTER TABLE dbo.ProductPrices ADD PriceType TINYINT NOT NULL CONSTRAINT DF_ProductPrices_PriceType DEFAULT (1); -- 1=Retail,2=Wholesale,3=Online,4=POS promo

IF COL_LENGTH('dbo.ProductPrices', 'Price') IS NULL
    ALTER TABLE dbo.ProductPrices ADD Price DECIMAL(18,3) NULL;

IF COL_LENGTH('dbo.ProductPrices', 'MinQty') IS NULL
    ALTER TABLE dbo.ProductPrices ADD MinQty DECIMAL(18,6) NULL;

IF COL_LENGTH('dbo.ProductPrices', 'EffectiveFrom') IS NULL
    ALTER TABLE dbo.ProductPrices ADD EffectiveFrom DATETIME2(0) NOT NULL CONSTRAINT DF_ProductPrices_EffectiveFrom DEFAULT (SYSDATETIME());

IF COL_LENGTH('dbo.ProductPrices', 'EffectiveTo') IS NULL
    ALTER TABLE dbo.ProductPrices ADD EffectiveTo DATETIME2(0) NULL;

IF COL_LENGTH('dbo.ProductPrices', 'IsActive') IS NULL
    ALTER TABLE dbo.ProductPrices ADD IsActive BIT NOT NULL CONSTRAINT DF_ProductPrices_IsActive DEFAULT (1);

-- Backfill existing data (legacy columns)
UPDATE dbo.ProductPrices
SET
    UomId = ISNULL(UomId, DefaultSellUomId),
    Price = ISNULL(Price, SellPrice),
    MinQty = ISNULL(MinQty, 1)
WHERE UomId IS NULL OR Price IS NULL OR MinQty IS NULL;

ALTER TABLE dbo.ProductPrices ALTER COLUMN UomId INT NOT NULL;
ALTER TABLE dbo.ProductPrices ALTER COLUMN Price DECIMAL(18,3) NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ProductPrices_Branches')
    ALTER TABLE dbo.ProductPrices WITH NOCHECK
        ADD CONSTRAINT FK_ProductPrices_Branches FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ProductPrices_Uoms')
    ALTER TABLE dbo.ProductPrices WITH NOCHECK
        ADD CONSTRAINT FK_ProductPrices_Uoms FOREIGN KEY (UomId) REFERENCES dbo.Uoms(Id);

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_ProductPrices' AND object_id = OBJECT_ID('dbo.ProductPrices'))
    DROP INDEX UQ_ProductPrices ON dbo.ProductPrices;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_ProductPrices' AND object_id = OBJECT_ID('dbo.ProductPrices'))
    CREATE UNIQUE INDEX UQ_ProductPrices
        ON dbo.ProductPrices (TenantId, ProductId, BranchId, PriceType, UomId, MinQty, EffectiveFrom);

-- Legacy tax/vat moved to product-level; keep columns if you still need backward compatibility.

/* -------------------------------
   PRODUCT UOMS: optional barcode per pack/UOM
-------------------------------- */
IF COL_LENGTH('dbo.ProductUoms', 'Barcode') IS NULL
    ALTER TABLE dbo.ProductUoms ADD Barcode NVARCHAR(50) NULL;

/* -------------------------------
   MULTI-CATEGORY TABLES
-------------------------------- */
IF OBJECT_ID('dbo.ProductCategories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductCategories
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductCategories PRIMARY KEY,
        TenantId INT NOT NULL,
        ParentCategoryId INT NULL,
        Code NVARCHAR(30) NOT NULL,
        Name NVARCHAR(150) NOT NULL,
        NameArabic NVARCHAR(150) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_ProductCategories_IsActive DEFAULT (1),
        CONSTRAINT UQ_ProductCategories_Code UNIQUE (TenantId, Code),
        CONSTRAINT FK_ProductCategories_Parent FOREIGN KEY (ParentCategoryId) REFERENCES dbo.ProductCategories(Id)
    );

    CREATE INDEX IX_ProductCategories_Name ON dbo.ProductCategories (TenantId, Name);
END;

IF OBJECT_ID('dbo.ProductCategoryMaps', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductCategoryMaps
    (
        Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductCategoryMaps PRIMARY KEY,
        TenantId INT NOT NULL,
        ProductId BIGINT NOT NULL,
        CategoryId INT NOT NULL,
        IsPrimary BIT NOT NULL CONSTRAINT DF_ProductCategoryMaps_IsPrimary DEFAULT (0),
        CONSTRAINT UQ_ProductCategoryMaps UNIQUE (TenantId, ProductId, CategoryId),
        CONSTRAINT FK_ProductCategoryMaps_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
        CONSTRAINT FK_ProductCategoryMaps_ProductCategories FOREIGN KEY (CategoryId) REFERENCES dbo.ProductCategories(Id)
    );
END;

/* -------------------------------
   BRANCH-WISE STOCK SNAPSHOT
-------------------------------- */
IF OBJECT_ID('dbo.ProductBranchStocks', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductBranchStocks
    (
        Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductBranchStocks PRIMARY KEY,
        TenantId INT NOT NULL,
        ProductId BIGINT NOT NULL,
        BranchId INT NOT NULL,
        OnHandQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_ProductBranchStocks_OnHandQty DEFAULT (0),
        ReservedQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_ProductBranchStocks_ReservedQty DEFAULT (0),
        ReorderLevel DECIMAL(18,6) NOT NULL CONSTRAINT DF_ProductBranchStocks_ReorderLevel DEFAULT (0),
        MaxStockLevel DECIMAL(18,6) NOT NULL CONSTRAINT DF_ProductBranchStocks_MaxStockLevel DEFAULT (0),
        LastUpdatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_ProductBranchStocks_LastUpdatedAt DEFAULT (SYSDATETIME()),
        CONSTRAINT UQ_ProductBranchStocks UNIQUE (TenantId, ProductId, BranchId),
        CONSTRAINT FK_ProductBranchStocks_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
        CONSTRAINT FK_ProductBranchStocks_Branches FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
    );
END;

COMMIT TRANSACTION;
