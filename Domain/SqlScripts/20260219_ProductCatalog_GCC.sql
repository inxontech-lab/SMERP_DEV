/*
    GCC-focused inventory schema bootstrap script (multi-tenant + Arabic support)
    - Removes legacy product tables under dbo
    - Creates inventory schema and core inventory module tables
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

/* -------------------------------
   1) Remove legacy product tables
-------------------------------- */
IF OBJECT_ID('dbo.ProductBranchStocks', 'U') IS NOT NULL DROP TABLE dbo.ProductBranchStocks;
IF OBJECT_ID('dbo.ProductCategoryMaps', 'U') IS NOT NULL DROP TABLE dbo.ProductCategoryMaps;
IF OBJECT_ID('dbo.ProductPrices', 'U') IS NOT NULL DROP TABLE dbo.ProductPrices;
IF OBJECT_ID('dbo.ProductUoms', 'U') IS NOT NULL DROP TABLE dbo.ProductUoms;
IF OBJECT_ID('dbo.ProductCategories', 'U') IS NOT NULL DROP TABLE dbo.ProductCategories;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;

/* -------------------------------
   2) Ensure inventory schema exists
-------------------------------- */
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'inventory')
    EXEC ('CREATE SCHEMA inventory');

/* -------------------------------
   3) Master tables
-------------------------------- */
IF OBJECT_ID('inventory.Suppliers', 'U') IS NOT NULL DROP TABLE inventory.Suppliers;
CREATE TABLE inventory.Suppliers
(
    SupplierId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_Suppliers PRIMARY KEY,
    TenantId INT NOT NULL,
    SupplierCode NVARCHAR(30) NOT NULL,
    SupplierName NVARCHAR(200) NOT NULL,
    SupplierNameArabic NVARCHAR(200) NULL,
    ContactPerson NVARCHAR(150) NULL,
    ContactPersonArabic NVARCHAR(150) NULL,
    PhoneNo NVARCHAR(30) NULL,
    MobileNo NVARCHAR(30) NULL,
    Email NVARCHAR(150) NULL,
    VATRegistrationNo NVARCHAR(50) NULL,
    CRNo NVARCHAR(50) NULL,
    AddressLine1 NVARCHAR(250) NULL,
    AddressLine1Arabic NVARCHAR(250) NULL,
    AddressLine2 NVARCHAR(250) NULL,
    AddressLine2Arabic NVARCHAR(250) NULL,
    City NVARCHAR(100) NULL,
    CityArabic NVARCHAR(100) NULL,
    CountryCode NVARCHAR(3) NULL,
    PaymentTermDays INT NOT NULL CONSTRAINT DF_inventory_Suppliers_PaymentTermDays DEFAULT (0),
    CreditLimit DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_Suppliers_CreditLimit DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Suppliers_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Suppliers_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Suppliers_Code UNIQUE (TenantId, SupplierCode)
);

IF OBJECT_ID('inventory.Units', 'U') IS NOT NULL DROP TABLE inventory.Units;
CREATE TABLE inventory.Units
(
    UnitId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_Units PRIMARY KEY,
    TenantId INT NOT NULL,
    UnitCode NVARCHAR(20) NOT NULL,
    UnitName NVARCHAR(100) NOT NULL,
    UnitNameArabic NVARCHAR(100) NULL,
    DecimalPrecision TINYINT NOT NULL CONSTRAINT DF_inventory_Units_DecimalPrecision DEFAULT (3),
    IsBaseUnit BIT NOT NULL CONSTRAINT DF_inventory_Units_IsBaseUnit DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Units_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Units_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Units_Code UNIQUE (TenantId, UnitCode)
);

IF OBJECT_ID('inventory.ItemCategories', 'U') IS NOT NULL DROP TABLE inventory.ItemCategories;
CREATE TABLE inventory.ItemCategories
(
    ItemCategoryId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_ItemCategories PRIMARY KEY,
    TenantId INT NOT NULL,
    CategoryCode NVARCHAR(30) NOT NULL,
    CategoryName NVARCHAR(150) NOT NULL,
    CategoryNameArabic NVARCHAR(150) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemCategories_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemCategories_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemCategories_Code UNIQUE (TenantId, CategoryCode)
);

IF OBJECT_ID('inventory.ItemSubCategories', 'U') IS NOT NULL DROP TABLE inventory.ItemSubCategories;
CREATE TABLE inventory.ItemSubCategories
(
    ItemSubCategoryId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_ItemSubCategories PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemCategoryId INT NOT NULL,
    SubCategoryCode NVARCHAR(30) NOT NULL,
    SubCategoryName NVARCHAR(150) NOT NULL,
    SubCategoryNameArabic NVARCHAR(150) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemSubCategories_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemSubCategories_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemSubCategories_Code UNIQUE (TenantId, ItemCategoryId, SubCategoryCode),
    CONSTRAINT FK_inventory_ItemSubCategories_Category FOREIGN KEY (ItemCategoryId)
        REFERENCES inventory.ItemCategories (ItemCategoryId)
);

IF OBJECT_ID('inventory.Items', 'U') IS NOT NULL DROP TABLE inventory.Items;
CREATE TABLE inventory.Items
(
    ItemId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_Items PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemCode NVARCHAR(40) NOT NULL,
    Barcode NVARCHAR(50) NULL,
    ItemName NVARCHAR(200) NOT NULL,
    ItemNameArabic NVARCHAR(200) NULL,
    ItemShortName NVARCHAR(100) NULL,
    ItemShortNameArabic NVARCHAR(100) NULL,
    Description NVARCHAR(1000) NULL,
    DescriptionArabic NVARCHAR(1000) NULL,
    ItemCategoryId INT NOT NULL,
    ItemSubCategoryId INT NULL,
    BaseUnitId INT NOT NULL,
    PurchaseUnitId INT NULL,
    SalesUnitId INT NULL,
    SupplierId BIGINT NULL,
    OriginCountryCode NVARCHAR(3) NULL,
    HSCode NVARCHAR(20) NULL,
    VATRate DECIMAL(5,2) NOT NULL CONSTRAINT DF_inventory_Items_VATRate DEFAULT (0),
    IsVATInclusive BIT NOT NULL CONSTRAINT DF_inventory_Items_IsVATInclusive DEFAULT (0),
    StandardCost DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_Items_StandardCost DEFAULT (0),
    LastPurchaseCost DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_Items_LastPurchaseCost DEFAULT (0),
    SellingPrice DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_Items_SellingPrice DEFAULT (0),
    ReorderLevel DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_Items_ReorderLevel DEFAULT (0),
    ReorderQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_Items_ReorderQty DEFAULT (0),
    MaxStockLevel DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_Items_MaxStockLevel DEFAULT (0),
    IsBatchTracked BIT NOT NULL CONSTRAINT DF_inventory_Items_IsBatchTracked DEFAULT (0),
    IsSerialTracked BIT NOT NULL CONSTRAINT DF_inventory_Items_IsSerialTracked DEFAULT (0),
    ExpiryDays INT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Items_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Items_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Items_Code UNIQUE (TenantId, ItemCode),
    CONSTRAINT FK_inventory_Items_Category FOREIGN KEY (ItemCategoryId)
        REFERENCES inventory.ItemCategories (ItemCategoryId),
    CONSTRAINT FK_inventory_Items_SubCategory FOREIGN KEY (ItemSubCategoryId)
        REFERENCES inventory.ItemSubCategories (ItemSubCategoryId),
    CONSTRAINT FK_inventory_Items_BaseUnit FOREIGN KEY (BaseUnitId)
        REFERENCES inventory.Units (UnitId),
    CONSTRAINT FK_inventory_Items_PurchaseUnit FOREIGN KEY (PurchaseUnitId)
        REFERENCES inventory.Units (UnitId),
    CONSTRAINT FK_inventory_Items_SalesUnit FOREIGN KEY (SalesUnitId)
        REFERENCES inventory.Units (UnitId),
    CONSTRAINT FK_inventory_Items_Supplier FOREIGN KEY (SupplierId)
        REFERENCES inventory.Suppliers (SupplierId)
);

IF OBJECT_ID('inventory.ItemUnitConversions', 'U') IS NOT NULL DROP TABLE inventory.ItemUnitConversions;
CREATE TABLE inventory.ItemUnitConversions
(
    ItemUnitConversionId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_ItemUnitConversions PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    FromUnitId INT NOT NULL,
    ToUnitId INT NOT NULL,
    ConversionFactor DECIMAL(18,6) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemUnitConversions_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemUnitConversions_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemUnitConversions UNIQUE (TenantId, ItemId, FromUnitId, ToUnitId),
    CONSTRAINT FK_inventory_ItemUnitConversions_Item FOREIGN KEY (ItemId)
        REFERENCES inventory.Items (ItemId),
    CONSTRAINT FK_inventory_ItemUnitConversions_FromUnit FOREIGN KEY (FromUnitId)
        REFERENCES inventory.Units (UnitId),
    CONSTRAINT FK_inventory_ItemUnitConversions_ToUnit FOREIGN KEY (ToUnitId)
        REFERENCES inventory.Units (UnitId)
);

/* -------------------------------
   4) Inventory transaction tables
   Design note:
   - inventory.InventoryTransactions is the source-of-truth stock ledger.
   - inventory.ItemStockBalances is a performance snapshot for fast reads.
   - inventory.vwItemStockRuntime provides pure runtime stock calculation.
-------------------------------- */
IF OBJECT_ID('inventory.Warehouses', 'U') IS NOT NULL DROP TABLE inventory.Warehouses;
CREATE TABLE inventory.Warehouses
(
    WarehouseId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_Warehouses PRIMARY KEY,
    TenantId INT NOT NULL,
    WarehouseCode NVARCHAR(30) NOT NULL,
    WarehouseName NVARCHAR(150) NOT NULL,
    WarehouseNameArabic NVARCHAR(150) NULL,
    Address NVARCHAR(300) NULL,
    AddressArabic NVARCHAR(300) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Warehouses_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Warehouses_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Warehouses_Code UNIQUE (TenantId, WarehouseCode)
);

IF OBJECT_ID('inventory.ItemStockBalances', 'U') IS NOT NULL DROP TABLE inventory.ItemStockBalances;
CREATE TABLE inventory.ItemStockBalances
(
    ItemStockBalanceId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_ItemStockBalances PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    OnHandQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_ItemStockBalances_OnHandQty DEFAULT (0),
    ReservedQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_ItemStockBalances_ReservedQty DEFAULT (0),
    AvailableQty AS (OnHandQty - ReservedQty) PERSISTED,
    AvgCost DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_ItemStockBalances_AvgCost DEFAULT (0),
    LastTransactionDate DATETIME2(0) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemStockBalances_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemStockBalances UNIQUE (TenantId, ItemId, WarehouseId, BatchNo, ExpiryDate),
    CONSTRAINT FK_inventory_ItemStockBalances_Item FOREIGN KEY (ItemId)
        REFERENCES inventory.Items (ItemId),
    CONSTRAINT FK_inventory_ItemStockBalances_Warehouse FOREIGN KEY (WarehouseId)
        REFERENCES inventory.Warehouses (WarehouseId)
);

IF OBJECT_ID('inventory.InventoryTransactions', 'U') IS NOT NULL DROP TABLE inventory.InventoryTransactions;
CREATE TABLE inventory.InventoryTransactions
(
    InventoryTransactionId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_InventoryTransactions PRIMARY KEY,
    TenantId INT NOT NULL,
    TransactionNo NVARCHAR(40) NOT NULL,
    TransactionType NVARCHAR(20) NOT NULL, -- GRN/ISSUE/TRANSFER/ADJUSTMENT/OPENING
    TransactionDate DATETIME2(0) NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    UnitId INT NOT NULL,
    QtyIn DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_InventoryTransactions_QtyIn DEFAULT (0),
    QtyOut DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_InventoryTransactions_QtyOut DEFAULT (0),
    UnitCost DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_InventoryTransactions_UnitCost DEFAULT (0),
    TotalCost AS ((QtyIn - QtyOut) * UnitCost) PERSISTED,
    ReferenceType NVARCHAR(30) NULL,
    ReferenceNo NVARCHAR(40) NULL,
    Remarks NVARCHAR(500) NULL,
    RemarksArabic NVARCHAR(500) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_InventoryTransactions_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_InventoryTransactions UNIQUE (TenantId, TransactionNo),
    CONSTRAINT FK_inventory_InventoryTransactions_Item FOREIGN KEY (ItemId)
        REFERENCES inventory.Items (ItemId),
    CONSTRAINT FK_inventory_InventoryTransactions_Warehouse FOREIGN KEY (WarehouseId)
        REFERENCES inventory.Warehouses (WarehouseId),
    CONSTRAINT FK_inventory_InventoryTransactions_Unit FOREIGN KEY (UnitId)
        REFERENCES inventory.Units (UnitId)
);

/* Runtime stock calculation view (no stored balance dependency) */
IF OBJECT_ID('inventory.vwItemStockRuntime', 'V') IS NOT NULL DROP VIEW inventory.vwItemStockRuntime;
EXEC ('
CREATE VIEW inventory.vwItemStockRuntime
AS
SELECT
    it.TenantId,
    it.ItemId,
    it.WarehouseId,
    CAST(NULL AS NVARCHAR(60)) AS BatchNo,
    CAST(NULL AS DATE) AS ExpiryDate,
    SUM(it.QtyIn - it.QtyOut) AS RuntimeOnHandQty,
    MAX(it.TransactionDate) AS LastTransactionDate
FROM inventory.InventoryTransactions it
GROUP BY
    it.TenantId,
    it.ItemId,
    it.WarehouseId
');

/* -------------------------------
   5) Purchase receipt (GRN)
-------------------------------- */
IF OBJECT_ID('inventory.GoodsReceiptLines', 'U') IS NOT NULL DROP TABLE inventory.GoodsReceiptLines;
IF OBJECT_ID('inventory.GoodsReceiptHeaders', 'U') IS NOT NULL DROP TABLE inventory.GoodsReceiptHeaders;

CREATE TABLE inventory.GoodsReceiptHeaders
(
    GoodsReceiptHeaderId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_GoodsReceiptHeaders PRIMARY KEY,
    TenantId INT NOT NULL,
    GRNNo NVARCHAR(40) NOT NULL,
    GRNDate DATETIME2(0) NOT NULL,
    SupplierId BIGINT NOT NULL,
    SupplierInvoiceNo NVARCHAR(60) NULL,
    SupplierInvoiceDate DATE NULL,
    CurrencyCode NVARCHAR(3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_CurrencyCode DEFAULT ('AED'),
    ExchangeRate DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_ExchangeRate DEFAULT (1),
    SubTotal DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_SubTotal DEFAULT (0),
    VATAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_VATAmount DEFAULT (0),
    NetTotal DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_NetTotal DEFAULT (0),
    Remarks NVARCHAR(500) NULL,
    RemarksArabic NVARCHAR(500) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_GoodsReceiptHeaders_GRNNo UNIQUE (TenantId, GRNNo),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Supplier FOREIGN KEY (SupplierId)
        REFERENCES inventory.Suppliers (SupplierId)
);

CREATE TABLE inventory.GoodsReceiptLines
(
    GoodsReceiptLineId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_inventory_GoodsReceiptLines PRIMARY KEY,
    TenantId INT NOT NULL,
    GoodsReceiptHeaderId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    UnitId INT NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    ReceivedQty DECIMAL(18,6) NOT NULL,
    UnitCost DECIMAL(18,3) NOT NULL,
    DiscountAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_DiscountAmount DEFAULT (0),
    VATAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_VATAmount DEFAULT (0),
    LineTotal DECIMAL(18,3) NOT NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_GoodsReceiptLines_Header FOREIGN KEY (GoodsReceiptHeaderId)
        REFERENCES inventory.GoodsReceiptHeaders (GoodsReceiptHeaderId),
    CONSTRAINT FK_inventory_GoodsReceiptLines_Item FOREIGN KEY (ItemId)
        REFERENCES inventory.Items (ItemId),
    CONSTRAINT FK_inventory_GoodsReceiptLines_Warehouse FOREIGN KEY (WarehouseId)
        REFERENCES inventory.Warehouses (WarehouseId),
    CONSTRAINT FK_inventory_GoodsReceiptLines_Unit FOREIGN KEY (UnitId)
        REFERENCES inventory.Units (UnitId)
);

/* -------------------------------
   6) Recommended indexes
-------------------------------- */
CREATE INDEX IX_inventory_Suppliers_Tenant_NameArabic ON inventory.Suppliers (TenantId, SupplierNameArabic);
CREATE INDEX IX_inventory_Items_Tenant_NameArabic ON inventory.Items (TenantId, ItemNameArabic);
CREATE INDEX IX_inventory_InventoryTransactions_Tenant_Date ON inventory.InventoryTransactions (TenantId, TransactionDate);
CREATE INDEX IX_inventory_ItemStockBalances_Tenant_ItemWarehouse ON inventory.ItemStockBalances (TenantId, ItemId, WarehouseId);

COMMIT TRANSACTION;
