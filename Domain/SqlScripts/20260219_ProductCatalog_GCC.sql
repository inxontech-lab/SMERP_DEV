IF OBJECT_ID('inventory.AuditLogs', 'U') IS NOT NULL DROP TABLE inventory.AuditLogs;
CREATE TABLE inventory.AuditLogs
(
    AuditLogId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ModuleName NVARCHAR(100) NOT NULL,
    EntityName NVARCHAR(120) NOT NULL,
    EntityId NVARCHAR(120) NULL,
    ActionType NVARCHAR(30) NOT NULL, -- INSERT/UPDATE/DELETE/POST/REVERSE
    OldData NVARCHAR(MAX) NULL,
    NewData NVARCHAR(MAX) NULL,
    IPAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(300) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_AuditLogs_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL
);

IF OBJECT_ID('inventory.Currencies', 'U') IS NOT NULL DROP TABLE inventory.Currencies;
CREATE TABLE inventory.Currencies
(
    CurrencyId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    CurrencyCode NVARCHAR(3) NOT NULL,
    CurrencyName NVARCHAR(60) NOT NULL,
    CurrencyNameArabic NVARCHAR(60) NULL,
    Symbol NVARCHAR(10) NULL,
    IsBaseCurrency BIT NOT NULL CONSTRAINT DF_inventory_Currencies_IsBaseCurrency DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Currencies_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Currencies_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Currencies UNIQUE (TenantId, CurrencyCode)
);

IF OBJECT_ID('inventory.ExchangeRates', 'U') IS NOT NULL DROP TABLE inventory.ExchangeRates;
CREATE TABLE inventory.ExchangeRates
(
    ExchangeRateId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    CurrencyId INT NOT NULL,
    RateDate DATE NOT NULL,
    BuyRate DECIMAL(18,6) NOT NULL,
    SellRate DECIMAL(18,6) NOT NULL,
    AverageRate DECIMAL(18,6) NOT NULL,
    SourceName NVARCHAR(100) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ExchangeRates_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ExchangeRates UNIQUE (TenantId, CurrencyId, RateDate),
    CONSTRAINT FK_inventory_ExchangeRates_Currency FOREIGN KEY (CurrencyId) REFERENCES inventory.Currencies(CurrencyId)
);

IF OBJECT_ID('inventory.TaxCodes', 'U') IS NOT NULL DROP TABLE inventory.TaxCodes;
CREATE TABLE inventory.TaxCodes
(
    TaxCodeId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    TaxCode NVARCHAR(20) NOT NULL,
    TaxName NVARCHAR(100) NOT NULL,
    TaxNameArabic NVARCHAR(100) NULL,
    TaxType NVARCHAR(20) NOT NULL, -- VAT/EXCISE/ZERO/EXEMPT
    TaxRate DECIMAL(5,2) NOT NULL,
    IsInclusive BIT NOT NULL CONSTRAINT DF_inventory_TaxCodes_IsInclusive DEFAULT (0),
    CountryCode NVARCHAR(3) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_TaxCodes_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_TaxCodes_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_TaxCodes UNIQUE (TenantId, TaxCode)
);

/* =========================================================
   2) MASTER DATA
IF OBJECT_ID('inventory.Suppliers', 'U') IS NOT NULL DROP TABLE inventory.Suppliers;
CREATE TABLE inventory.Suppliers
(
    SupplierId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    SupplierCode NVARCHAR(30) NOT NULL,
    SupplierName NVARCHAR(200) NOT NULL,
    SupplierNameArabic NVARCHAR(200) NULL,
    ContactPerson NVARCHAR(120) NULL,
    ContactPersonArabic NVARCHAR(120) NULL,
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
    IsApprovedVendor BIT NOT NULL CONSTRAINT DF_inventory_Suppliers_IsApprovedVendor DEFAULT (1),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Suppliers_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Suppliers_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Suppliers UNIQUE (TenantId, SupplierCode)
);

IF OBJECT_ID('inventory.Units', 'U') IS NOT NULL DROP TABLE inventory.Units;
CREATE TABLE inventory.Units
(
    UnitId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    UnitCode NVARCHAR(20) NOT NULL,
    UnitName NVARCHAR(100) NOT NULL,
    UnitNameArabic NVARCHAR(100) NULL,
    DecimalPrecision TINYINT NOT NULL CONSTRAINT DF_inventory_Units_DecimalPrecision DEFAULT (3),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Units_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Units_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Units UNIQUE (TenantId, UnitCode)
);

IF OBJECT_ID('inventory.ItemCategories', 'U') IS NOT NULL DROP TABLE inventory.ItemCategories;
CREATE TABLE inventory.ItemCategories
(
    ItemCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    CategoryCode NVARCHAR(30) NOT NULL,
    CategoryName NVARCHAR(150) NOT NULL,
    CategoryNameArabic NVARCHAR(150) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemCategories_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemCategories_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemCategories UNIQUE (TenantId, CategoryCode)
);

IF OBJECT_ID('inventory.ItemSubCategories', 'U') IS NOT NULL DROP TABLE inventory.ItemSubCategories;
CREATE TABLE inventory.ItemSubCategories
(
    ItemSubCategoryId INT IDENTITY(1,1) PRIMARY KEY,
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
    CONSTRAINT UQ_inventory_ItemSubCategories UNIQUE (TenantId, ItemCategoryId, SubCategoryCode),
    CONSTRAINT FK_inventory_ItemSubCategories_Category FOREIGN KEY (ItemCategoryId) REFERENCES inventory.ItemCategories(ItemCategoryId)
);

IF OBJECT_ID('inventory.Brands', 'U') IS NOT NULL DROP TABLE inventory.Brands;
CREATE TABLE inventory.Brands
(
    BrandId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BrandCode NVARCHAR(30) NOT NULL,
    BrandName NVARCHAR(120) NOT NULL,
    BrandNameArabic NVARCHAR(120) NULL,
    CountryCode NVARCHAR(3) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Brands_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Brands_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Brands UNIQUE (TenantId, BrandCode)
);

IF OBJECT_ID('inventory.Items', 'U') IS NOT NULL DROP TABLE inventory.Items;
CREATE TABLE inventory.Items
(
    ItemId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemCode NVARCHAR(40) NOT NULL,
    SKU NVARCHAR(40) NULL,
    Barcode NVARCHAR(50) NULL,
    ItemName NVARCHAR(200) NOT NULL,
    ItemNameArabic NVARCHAR(200) NULL,
    ItemShortName NVARCHAR(100) NULL,
    ItemShortNameArabic NVARCHAR(100) NULL,
    Description NVARCHAR(1000) NULL,
    DescriptionArabic NVARCHAR(1000) NULL,
    BrandId INT NULL,
    ItemCategoryId INT NOT NULL,
    ItemSubCategoryId INT NULL,
    BaseUnitId INT NOT NULL,
    SalesUnitId INT NULL,
    PurchaseUnitId INT NULL,
    TaxCodeId INT NULL,
    PreferredSupplierId BIGINT NULL,
    OriginCountryCode NVARCHAR(3) NULL,
    HSCode NVARCHAR(20) NULL,
    IsBatchTracked BIT NOT NULL CONSTRAINT DF_inventory_Items_IsBatchTracked DEFAULT (0),
    IsSerialTracked BIT NOT NULL CONSTRAINT DF_inventory_Items_IsSerialTracked DEFAULT (0),
    IsExpiryTracked BIT NOT NULL CONSTRAINT DF_inventory_Items_IsExpiryTracked DEFAULT (0),
    ShelfLifeDays INT NULL,
    MinSellingPrice DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_Items_MinSellingPrice DEFAULT (0),
    StandardCost DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_Items_StandardCost DEFAULT (0),
    IsHalalCertified BIT NOT NULL CONSTRAINT DF_inventory_Items_IsHalalCertified DEFAULT (0), -- GCC unique
    RamadanDemandFactor DECIMAL(6,3) NOT NULL CONSTRAINT DF_inventory_Items_RamadanDemandFactor DEFAULT (1.000), -- unique demand tuning
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Items_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Items_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Items UNIQUE (TenantId, ItemCode),
    CONSTRAINT FK_inventory_Items_Brand FOREIGN KEY (BrandId) REFERENCES inventory.Brands(BrandId),
    CONSTRAINT FK_inventory_Items_Category FOREIGN KEY (ItemCategoryId) REFERENCES inventory.ItemCategories(ItemCategoryId),
    CONSTRAINT FK_inventory_Items_SubCategory FOREIGN KEY (ItemSubCategoryId) REFERENCES inventory.ItemSubCategories(ItemSubCategoryId),
    CONSTRAINT FK_inventory_Items_BaseUnit FOREIGN KEY (BaseUnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_Items_SalesUnit FOREIGN KEY (SalesUnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_Items_PurchaseUnit FOREIGN KEY (PurchaseUnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_Items_TaxCode FOREIGN KEY (TaxCodeId) REFERENCES inventory.TaxCodes(TaxCodeId),
    CONSTRAINT FK_inventory_Items_Supplier FOREIGN KEY (PreferredSupplierId) REFERENCES inventory.Suppliers(SupplierId)
);

IF OBJECT_ID('inventory.ItemBarcodes', 'U') IS NOT NULL DROP TABLE inventory.ItemBarcodes;
CREATE TABLE inventory.ItemBarcodes
(
    ItemBarcodeId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    Barcode NVARCHAR(50) NOT NULL,
    IsPrimary BIT NOT NULL CONSTRAINT DF_inventory_ItemBarcodes_IsPrimary DEFAULT (0),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemBarcodes_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemBarcodes UNIQUE (TenantId, Barcode),
    CONSTRAINT FK_inventory_ItemBarcodes_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemBarcodes_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId)
);

IF OBJECT_ID('inventory.ItemSuppliers', 'U') IS NOT NULL DROP TABLE inventory.ItemSuppliers;
CREATE TABLE inventory.ItemSuppliers
(
    ItemSupplierId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    SupplierId BIGINT NOT NULL,
    SupplierItemCode NVARCHAR(50) NULL,
    LastPurchasePrice DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_ItemSuppliers_LastPurchasePrice DEFAULT (0),
    LeadTimeDays INT NOT NULL CONSTRAINT DF_inventory_ItemSuppliers_LeadTimeDays DEFAULT (0),
    IsPreferred BIT NOT NULL CONSTRAINT DF_inventory_ItemSuppliers_IsPreferred DEFAULT (0),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemSuppliers_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemSuppliers UNIQUE (TenantId, ItemId, SupplierId),
    CONSTRAINT FK_inventory_ItemSuppliers_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemSuppliers_Supplier FOREIGN KEY (SupplierId) REFERENCES inventory.Suppliers(SupplierId)
);

IF OBJECT_ID('inventory.ItemUnitConversions', 'U') IS NOT NULL DROP TABLE inventory.ItemUnitConversions;
CREATE TABLE inventory.ItemUnitConversions
(
    ItemUnitConversionId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    FromUnitId INT NOT NULL,
    ToUnitId INT NOT NULL,
    ConversionFactor DECIMAL(18,6) NOT NULL,
    RoundingMode NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_ItemUnitConversions_RoundingMode DEFAULT ('ROUND'),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemUnitConversions_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemUnitConversions_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemUnitConversions UNIQUE (TenantId, ItemId, FromUnitId, ToUnitId),
    CONSTRAINT FK_inventory_ItemUnitConversions_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemUnitConversions_From FOREIGN KEY (FromUnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_ItemUnitConversions_To FOREIGN KEY (ToUnitId) REFERENCES inventory.Units(UnitId)
);

IF OBJECT_ID('dbo.Branches', 'U') IS NULL
    THROW 50001, 'dbo.Branches table is required for inventory script.', 1;

IF COL_LENGTH('dbo.Branches', 'IsWarehouse') IS NULL
BEGIN
    ALTER TABLE dbo.Branches
    ADD IsWarehouse BIT NOT NULL
        CONSTRAINT DF_dbo_Branches_IsWarehouse DEFAULT (0);
END;

IF OBJECT_ID('inventory.Warehouses', 'U') IS NOT NULL DROP TABLE inventory.Warehouses;
CREATE TABLE inventory.Warehouses
(
    WarehouseId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    WarehouseCode NVARCHAR(30) NOT NULL,
    WarehouseName NVARCHAR(150) NOT NULL,
    WarehouseNameArabic NVARCHAR(150) NULL,
    Address NVARCHAR(300) NULL,
    AddressArabic NVARCHAR(300) NULL,
    City NVARCHAR(100) NULL,
    CityArabic NVARCHAR(100) NULL,
    TemperatureZone NVARCHAR(30) NULL,
    IsTransitWarehouse BIT NOT NULL CONSTRAINT DF_inventory_Warehouses_IsTransitWarehouse DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Warehouses_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Warehouses_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Warehouses UNIQUE (TenantId, BranchId, WarehouseCode),
    CONSTRAINT FK_inventory_Warehouses_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.WarehouseBins', 'U') IS NOT NULL DROP TABLE inventory.WarehouseBins;
CREATE TABLE inventory.WarehouseBins
(
    WarehouseBinId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    WarehouseId INT NOT NULL,
    BinCode NVARCHAR(40) NOT NULL,
    BinName NVARCHAR(120) NOT NULL,
    BinNameArabic NVARCHAR(120) NULL,
    IsPickFace BIT NOT NULL CONSTRAINT DF_inventory_WarehouseBins_IsPickFace DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_WarehouseBins_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_WarehouseBins_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_WarehouseBins UNIQUE (TenantId, WarehouseId, BinCode),
    CONSTRAINT FK_inventory_WarehouseBins_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId)
);

IF OBJECT_ID('inventory.PriceLists', 'U') IS NOT NULL DROP TABLE inventory.PriceLists;
CREATE TABLE inventory.PriceLists
(
    PriceListId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    PriceListCode NVARCHAR(30) NOT NULL,
    PriceListName NVARCHAR(150) NOT NULL,
    PriceListNameArabic NVARCHAR(150) NULL,
    PriceListType NVARCHAR(20) NOT NULL, -- RETAIL/WHOLESALE/ONLINE/PROMO
    CurrencyId INT NOT NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_PriceLists_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_PriceLists_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_PriceLists UNIQUE (TenantId, PriceListCode),
    CONSTRAINT FK_inventory_PriceLists_Currency FOREIGN KEY (CurrencyId) REFERENCES inventory.Currencies(CurrencyId)
);

IF OBJECT_ID('inventory.ItemPrices', 'U') IS NOT NULL DROP TABLE inventory.ItemPrices;
CREATE TABLE inventory.ItemPrices
(
    ItemPriceId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    PriceListId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    MinQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_ItemPrices_MinQty DEFAULT (1),
    Price DECIMAL(18,3) NOT NULL,
    DiscountPercent DECIMAL(5,2) NOT NULL CONSTRAINT DF_inventory_ItemPrices_DiscountPercent DEFAULT (0),
    TaxCodeId INT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemPrices_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemPrices_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemPrices UNIQUE (TenantId, PriceListId, ItemId, UnitId, MinQty),
    CONSTRAINT FK_inventory_ItemPrices_PriceList FOREIGN KEY (PriceListId) REFERENCES inventory.PriceLists(PriceListId),
    CONSTRAINT FK_inventory_ItemPrices_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemPrices_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_ItemPrices_TaxCode FOREIGN KEY (TaxCodeId) REFERENCES inventory.TaxCodes(TaxCodeId)
);

IF OBJECT_ID('inventory.ReorderPolicies', 'U') IS NOT NULL DROP TABLE inventory.ReorderPolicies;
CREATE TABLE inventory.ReorderPolicies
(
    ReorderPolicyId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    MinStock DECIMAL(18,6) NOT NULL,
    MaxStock DECIMAL(18,6) NOT NULL,
    ReorderQty DECIMAL(18,6) NOT NULL,
    SafetyDays INT NOT NULL CONSTRAINT DF_inventory_ReorderPolicies_SafetyDays DEFAULT (7),
    SeasonalityMultiplier DECIMAL(6,3) NOT NULL CONSTRAINT DF_inventory_ReorderPolicies_SeasonalityMultiplier DEFAULT (1.000),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ReorderPolicies_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ReorderPolicies UNIQUE (TenantId, BranchId, ItemId, WarehouseId),
    CONSTRAINT FK_inventory_ReorderPolicies_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ReorderPolicies_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_ReorderPolicies_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

/* =========================================================
   3) STOCK + MOVEMENT TABLES
IF OBJECT_ID('inventory.StockLedger', 'U') IS NOT NULL DROP TABLE inventory.StockLedger;
CREATE TABLE inventory.StockLedger
(
    StockLedgerId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    LedgerDate DATETIME2(0) NOT NULL,
    TransactionType NVARCHAR(30) NOT NULL,
    TransactionNo NVARCHAR(40) NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    WarehouseBinId BIGINT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    SerialNo NVARCHAR(100) NULL,
    QtyIn DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_StockLedger_QtyIn DEFAULT (0),
    QtyOut DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_StockLedger_QtyOut DEFAULT (0),
    UnitCost DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_StockLedger_UnitCost DEFAULT (0),
    ValueImpact AS ((QtyIn - QtyOut) * UnitCost) PERSISTED,
    ReferenceType NVARCHAR(30) NULL,
    ReferenceId NVARCHAR(40) NULL,
    Notes NVARCHAR(500) NULL,
    NotesArabic NVARCHAR(500) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockLedger_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_StockLedger_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_StockLedger_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockLedger_Bin FOREIGN KEY (WarehouseBinId) REFERENCES inventory.WarehouseBins(WarehouseBinId),
    CONSTRAINT FK_inventory_StockLedger_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.ItemStockBalances', 'U') IS NOT NULL DROP TABLE inventory.ItemStockBalances;
CREATE TABLE inventory.ItemStockBalances
(
    ItemStockBalanceId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    WarehouseBinId BIGINT NULL,
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
    CONSTRAINT UQ_inventory_ItemStockBalances UNIQUE (TenantId, BranchId, ItemId, WarehouseId, WarehouseBinId, BatchNo, ExpiryDate),
    CONSTRAINT FK_inventory_ItemStockBalances_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemStockBalances_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_ItemStockBalances_Bin FOREIGN KEY (WarehouseBinId) REFERENCES inventory.WarehouseBins(WarehouseBinId),
    CONSTRAINT FK_inventory_ItemStockBalances_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.StockReservations', 'U') IS NOT NULL DROP TABLE inventory.StockReservations;
CREATE TABLE inventory.StockReservations
(
    StockReservationId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ReservationNo NVARCHAR(40) NOT NULL,
    ReservationType NVARCHAR(20) NOT NULL, -- SALES_ORDER/TRANSFER/PICKLIST
    ReservationDate DATETIME2(0) NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ReservedQty DECIMAL(18,6) NOT NULL,
    ReleasedQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_StockReservations_ReleasedQty DEFAULT (0),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_StockReservations_Status DEFAULT ('OPEN'),
    Remarks NVARCHAR(500) NULL,
    RemarksArabic NVARCHAR(500) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockReservations_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_StockReservations UNIQUE (TenantId, ReservationNo),
    CONSTRAINT FK_inventory_StockReservations_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_StockReservations_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockReservations_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.ItemBatches', 'U') IS NOT NULL DROP TABLE inventory.ItemBatches;
CREATE TABLE inventory.ItemBatches
(
    ItemBatchId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    BatchNo NVARCHAR(60) NOT NULL,
    MfgDate DATE NULL,
    ExpiryDate DATE NULL,
    CountryOfOrigin NVARCHAR(3) NULL,
    SupplierId BIGINT NULL,
    QtyOnHand DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_ItemBatches_QtyOnHand DEFAULT (0),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_ItemBatches_Status DEFAULT ('ACTIVE'),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemBatches_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemBatches UNIQUE (TenantId, ItemId, WarehouseId, BatchNo),
    CONSTRAINT FK_inventory_ItemBatches_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemBatches_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_ItemBatches_Supplier FOREIGN KEY (SupplierId) REFERENCES inventory.Suppliers(SupplierId),
    CONSTRAINT FK_inventory_ItemBatches_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.ItemSerials', 'U') IS NOT NULL DROP TABLE inventory.ItemSerials;
CREATE TABLE inventory.ItemSerials
(
    ItemSerialId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    SerialNo NVARCHAR(100) NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    WarrantyStartDate DATE NULL,
    WarrantyEndDate DATE NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_ItemSerials_Status DEFAULT ('IN_STOCK'),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemSerials_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemSerials UNIQUE (TenantId, SerialNo),
    CONSTRAINT FK_inventory_ItemSerials_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemSerials_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_ItemSerials_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

/* =========================================================
   4) PROCUREMENT + INTERNAL MOVEMENT
IF OBJECT_ID('inventory.GoodsReceiptHeaders', 'U') IS NOT NULL DROP TABLE inventory.GoodsReceiptHeaders;
IF OBJECT_ID('inventory.GoodsReceiptLines', 'U') IS NOT NULL DROP TABLE inventory.GoodsReceiptLines;
CREATE TABLE inventory.GoodsReceiptHeaders
(
    GoodsReceiptHeaderId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    GRNNo NVARCHAR(40) NOT NULL,
    GRNDate DATETIME2(0) NOT NULL,
    SupplierId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    CurrencyId INT NOT NULL,
    ExchangeRate DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_ExchangeRate DEFAULT (1),
    SupplierInvoiceNo NVARCHAR(60) NULL,
    SupplierInvoiceDate DATE NULL,
    SubTotal DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_SubTotal DEFAULT (0),
    DiscountAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_DiscountAmount DEFAULT (0),
    VATAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_VATAmount DEFAULT (0),
    ExciseAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_ExciseAmount DEFAULT (0),
    NetTotal DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_NetTotal DEFAULT (0),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_Status DEFAULT ('DRAFT'),
    Remarks NVARCHAR(500) NULL,
    RemarksArabic NVARCHAR(500) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_GoodsReceiptHeaders UNIQUE (TenantId, GRNNo),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Supplier FOREIGN KEY (SupplierId) REFERENCES inventory.Suppliers(SupplierId),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Currency FOREIGN KEY (CurrencyId) REFERENCES inventory.Currencies(CurrencyId),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);
CREATE TABLE inventory.GoodsReceiptLines
(
    GoodsReceiptLineId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    GoodsReceiptHeaderId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    ReceivedQty DECIMAL(18,6) NOT NULL,
    UnitCost DECIMAL(18,3) NOT NULL,
    DiscountPercent DECIMAL(5,2) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_DiscountPercent DEFAULT (0),
    VATAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_VATAmount DEFAULT (0),
    ExciseAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_ExciseAmount DEFAULT (0),
    LineTotal DECIMAL(18,3) NOT NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_GoodsReceiptLines_Header FOREIGN KEY (GoodsReceiptHeaderId) REFERENCES inventory.GoodsReceiptHeaders(GoodsReceiptHeaderId),
    CONSTRAINT FK_inventory_GoodsReceiptLines_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_GoodsReceiptLines_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId)
);

IF OBJECT_ID('inventory.StockTransferHeaders', 'U') IS NOT NULL DROP TABLE inventory.StockTransferHeaders;
IF OBJECT_ID('inventory.StockTransferLines', 'U') IS NOT NULL DROP TABLE inventory.StockTransferLines;
CREATE TABLE inventory.StockTransferHeaders
(
    StockTransferHeaderId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    FromBranchId INT NOT NULL,
    ToBranchId INT NOT NULL,
    TransferNo NVARCHAR(40) NOT NULL,
    TransferDate DATETIME2(0) NOT NULL,
    FromWarehouseId INT NOT NULL,
    ToWarehouseId INT NOT NULL,
    TransferReason NVARCHAR(200) NULL,
    TransferReasonArabic NVARCHAR(200) NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_StockTransferHeaders_Status DEFAULT ('DRAFT'),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockTransferHeaders_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_StockTransferHeaders UNIQUE (TenantId, TransferNo),
    CONSTRAINT FK_inventory_StockTransferHeaders_FromWarehouse FOREIGN KEY (FromWarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockTransferHeaders_ToWarehouse FOREIGN KEY (ToWarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockTransferHeaders_FromBranch FOREIGN KEY (FromBranchId) REFERENCES dbo.Branches(Id),
    CONSTRAINT FK_inventory_StockTransferHeaders_ToBranch FOREIGN KEY (ToBranchId) REFERENCES dbo.Branches(Id)
);
CREATE TABLE inventory.StockTransferLines
(
    StockTransferLineId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    StockTransferHeaderId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    TransferQty DECIMAL(18,6) NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockTransferLines_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_StockTransferLines_Header FOREIGN KEY (StockTransferHeaderId) REFERENCES inventory.StockTransferHeaders(StockTransferHeaderId),
    CONSTRAINT FK_inventory_StockTransferLines_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_StockTransferLines_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId)
);

IF OBJECT_ID('inventory.StockAdjustmentHeaders', 'U') IS NOT NULL DROP TABLE inventory.StockAdjustmentHeaders;
IF OBJECT_ID('inventory.StockAdjustmentLines', 'U') IS NOT NULL DROP TABLE inventory.StockAdjustmentLines;
CREATE TABLE inventory.StockAdjustmentHeaders
(
    StockAdjustmentHeaderId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    AdjustmentNo NVARCHAR(40) NOT NULL,
    AdjustmentDate DATETIME2(0) NOT NULL,
    WarehouseId INT NOT NULL,
    AdjustmentReason NVARCHAR(200) NOT NULL,
    AdjustmentReasonArabic NVARCHAR(200) NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_StockAdjustmentHeaders_Status DEFAULT ('DRAFT'),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockAdjustmentHeaders_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_StockAdjustmentHeaders UNIQUE (TenantId, AdjustmentNo),
    CONSTRAINT FK_inventory_StockAdjustmentHeaders_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockAdjustmentHeaders_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);
CREATE TABLE inventory.StockAdjustmentLines
(
    StockAdjustmentLineId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    StockAdjustmentHeaderId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    SystemQty DECIMAL(18,6) NOT NULL,
    CountedQty DECIMAL(18,6) NOT NULL,
    DifferenceQty AS (CountedQty - SystemQty) PERSISTED,
    UnitCost DECIMAL(18,3) NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockAdjustmentLines_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_StockAdjustmentLines_Header FOREIGN KEY (StockAdjustmentHeaderId) REFERENCES inventory.StockAdjustmentHeaders(StockAdjustmentHeaderId),
    CONSTRAINT FK_inventory_StockAdjustmentLines_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_StockAdjustmentLines_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId)
);

/* =========================================================
   5) VIEWS
IF OBJECT_ID('inventory.vwCurrentStock', 'V') IS NOT NULL DROP VIEW inventory.vwCurrentStock;
EXEC ('
CREATE VIEW inventory.vwCurrentStock
AS
SELECT
    sb.TenantId,
    sb.ItemId,
    i.ItemCode,
    i.ItemName,
    i.ItemNameArabic,
    sb.BranchId,
    sb.WarehouseId,
    w.WarehouseCode,
    w.WarehouseName,
    sb.BatchNo,
    sb.ExpiryDate,
    sb.OnHandQty,
    sb.ReservedQty,
    sb.AvailableQty,
    sb.AvgCost,
    (sb.OnHandQty * sb.AvgCost) AS StockValue
FROM inventory.ItemStockBalances sb
JOIN inventory.Items i ON i.ItemId = sb.ItemId
JOIN inventory.Warehouses w ON w.WarehouseId = sb.WarehouseId
');

IF OBJECT_ID('inventory.vwItemStockRuntime', 'V') IS NOT NULL DROP VIEW inventory.vwItemStockRuntime;
EXEC ('
CREATE VIEW inventory.vwItemStockRuntime
AS
SELECT
    sl.TenantId,
    sl.ItemId,
    sl.BranchId,
    sl.WarehouseId,
    sl.BatchNo,
    sl.ExpiryDate,
    SUM(sl.QtyIn - sl.QtyOut) AS RuntimeOnHandQty,
    MAX(sl.LedgerDate) AS LastMovementDate
FROM inventory.StockLedger sl
GROUP BY sl.TenantId, sl.BranchId, sl.ItemId, sl.WarehouseId, sl.BatchNo, sl.ExpiryDate
');

IF OBJECT_ID('inventory.vwNearExpiryStock', 'V') IS NOT NULL DROP VIEW inventory.vwNearExpiryStock;
EXEC ('
CREATE VIEW inventory.vwNearExpiryStock
AS
SELECT
    sb.TenantId,
    sb.ItemId,
    i.ItemCode,
    i.ItemName,
    sb.BranchId,
    sb.WarehouseId,
    sb.BatchNo,
    sb.ExpiryDate,
    DATEDIFF(DAY, CAST(GETDATE() AS DATE), sb.ExpiryDate) AS DaysToExpiry,
    sb.OnHandQty
FROM inventory.ItemStockBalances sb
JOIN inventory.Items i ON i.ItemId = sb.ItemId
WHERE sb.ExpiryDate IS NOT NULL AND sb.OnHandQty > 0
');

IF OBJECT_ID('inventory.vwReorderSuggestions', 'V') IS NOT NULL DROP VIEW inventory.vwReorderSuggestions;
EXEC ('
CREATE VIEW inventory.vwReorderSuggestions
AS
SELECT
    rp.TenantId,
    rp.ItemId,
    i.ItemCode,
    i.ItemName,
    rp.WarehouseId,
    rp.MinStock,
    rp.MaxStock,
    ISNULL(cs.OnHandQty, 0) AS CurrentStock,
    CASE WHEN ISNULL(cs.OnHandQty, 0) < rp.MinStock THEN (rp.MaxStock - ISNULL(cs.OnHandQty, 0)) ELSE 0 END AS SuggestedOrderQty
FROM inventory.ReorderPolicies rp
JOIN inventory.Items i ON i.ItemId = rp.ItemId
OUTER APPLY
(
    SELECT SUM(sb.OnHandQty) AS OnHandQty
    FROM inventory.ItemStockBalances sb
    WHERE sb.TenantId = rp.TenantId
      AND sb.ItemId = rp.ItemId
      AND sb.BranchId = rp.BranchId
      AND sb.WarehouseId = rp.WarehouseId
) cs
');

/* =========================================================
   6) FUNCTIONS
IF OBJECT_ID('inventory.fnStockOnHand', 'FN') IS NOT NULL DROP FUNCTION inventory.fnStockOnHand;
EXEC ('
CREATE FUNCTION inventory.fnStockOnHand
(
    @TenantId INT,
    @ItemId BIGINT,
    @BranchId INT,
    @WarehouseId INT
)
RETURNS DECIMAL(18,6)
AS
BEGIN
    DECLARE @Qty DECIMAL(18,6);

    SELECT @Qty = ISNULL(SUM(sb.OnHandQty), 0)
    FROM inventory.ItemStockBalances sb
    WHERE sb.TenantId = @TenantId
      AND sb.ItemId = @ItemId
      AND sb.BranchId = @BranchId
      AND sb.WarehouseId = @WarehouseId;

    RETURN ISNULL(@Qty, 0);
END
');

IF OBJECT_ID('inventory.fnAvailableStock', 'FN') IS NOT NULL DROP FUNCTION inventory.fnAvailableStock;
EXEC ('
CREATE FUNCTION inventory.fnAvailableStock
(
    @TenantId INT,
    @ItemId BIGINT,
    @BranchId INT,
    @WarehouseId INT
)
RETURNS DECIMAL(18,6)
AS
BEGIN
    DECLARE @Qty DECIMAL(18,6);

    SELECT @Qty = ISNULL(SUM(sb.AvailableQty), 0)
    FROM inventory.ItemStockBalances sb
    WHERE sb.TenantId = @TenantId
      AND sb.ItemId = @ItemId
      AND sb.BranchId = @BranchId
      AND sb.WarehouseId = @WarehouseId;

    RETURN ISNULL(@Qty, 0);
END
');

/* =========================================================
   7) PROCEDURES
IF OBJECT_ID('inventory.spCreateAuditLog', 'P') IS NOT NULL DROP PROCEDURE inventory.spCreateAuditLog;
EXEC ('
CREATE PROCEDURE inventory.spCreateAuditLog
    @TenantId INT,
    @ModuleName NVARCHAR(100),
    @EntityName NVARCHAR(120),
    @EntityId NVARCHAR(120) = NULL,
    @ActionType NVARCHAR(30),
    @OldData NVARCHAR(MAX) = NULL,
    @NewData NVARCHAR(MAX) = NULL,
    @CreatedUserId BIGINT,
    @IPAddress NVARCHAR(45) = NULL,
    @UserAgent NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO inventory.AuditLogs
    (
        TenantId, ModuleName, EntityName, EntityId, ActionType,
        OldData, NewData, IPAddress, UserAgent,
        CreatedUserId, CreatedDateTime
    )
    VALUES
    (
        @TenantId, @ModuleName, @EntityName, @EntityId, @ActionType,
        @OldData, @NewData, @IPAddress, @UserAgent,
        @CreatedUserId, SYSDATETIME()
    );
END
');

IF OBJECT_ID('inventory.spRebuildStockBalances', 'P') IS NOT NULL DROP PROCEDURE inventory.spRebuildStockBalances;
EXEC ('
CREATE PROCEDURE inventory.spRebuildStockBalances
    @TenantId INT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM inventory.ItemStockBalances WHERE TenantId = @TenantId;

    INSERT INTO inventory.ItemStockBalances
    (
        TenantId, BranchId, ItemId, WarehouseId, WarehouseBinId, BatchNo, ExpiryDate,
        OnHandQty, ReservedQty, AvgCost, LastTransactionDate,
        CreatedUserId, CreatedDateTime
    )
    SELECT
        sl.TenantId,
        sl.BranchId,
        sl.ItemId,
        sl.WarehouseId,
        sl.WarehouseBinId,
        sl.BatchNo,
        sl.ExpiryDate,
        SUM(sl.QtyIn - sl.QtyOut) AS OnHandQty,
        0 AS ReservedQty,
        CASE WHEN SUM(sl.QtyIn - sl.QtyOut) = 0 THEN 0
             ELSE SUM((sl.QtyIn - sl.QtyOut) * sl.UnitCost) / NULLIF(SUM(sl.QtyIn - sl.QtyOut),0)
        END AS AvgCost,
        MAX(sl.LedgerDate) AS LastTransactionDate,
        @UserId,
        SYSDATETIME()
    FROM inventory.StockLedger sl
    WHERE sl.TenantId = @TenantId
    GROUP BY sl.TenantId, sl.BranchId, sl.ItemId, sl.WarehouseId, sl.WarehouseBinId, sl.BatchNo, sl.ExpiryDate;

    EXEC inventory.spCreateAuditLog
        @TenantId = @TenantId,
        @ModuleName = ''Inventory'',
        @EntityName = ''ItemStockBalances'',
        @ActionType = ''REBUILD'',
        @NewData = ''Stock balances rebuilt from ledger'',
        @CreatedUserId = @UserId;
END
');

IF OBJECT_ID('inventory.spReserveStock', 'P') IS NOT NULL DROP PROCEDURE inventory.spReserveStock;
EXEC ('
CREATE PROCEDURE inventory.spReserveStock
    @TenantId INT,
    @ReservationNo NVARCHAR(40),
    @ReservationType NVARCHAR(20),
    @ItemId BIGINT,
    @BranchId INT,
    @WarehouseId INT,
    @ReservedQty DECIMAL(18,6),
    @UserId BIGINT,
    @Remarks NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Available DECIMAL(18,6) = inventory.fnAvailableStock(@TenantId, @ItemId, @BranchId, @WarehouseId);

    IF @Available < @ReservedQty
        THROW 50001, ''Insufficient available stock for reservation.'', 1;

    INSERT INTO inventory.StockReservations
    (
        TenantId, ReservationNo, ReservationType, ReservationDate, ItemId, BranchId, WarehouseId,
        ReservedQty, Remarks, CreatedUserId, CreatedDateTime
    )
    VALUES
    (
        @TenantId, @ReservationNo, @ReservationType, SYSDATETIME(), @ItemId, @BranchId, @WarehouseId,
        @ReservedQty, @Remarks, @UserId, SYSDATETIME()
    );

    UPDATE sb
    SET sb.ReservedQty = sb.ReservedQty + @ReservedQty,
        sb.UpdatedUserId = @UserId,
        sb.UpdatedDateTime = SYSDATETIME()
    FROM inventory.ItemStockBalances sb
    WHERE sb.TenantId = @TenantId
      AND sb.ItemId = @ItemId
      AND sb.BranchId = @BranchId
      AND sb.WarehouseId = @WarehouseId;
END
');

IF OBJECT_ID('inventory.spPostGoodsReceipt', 'P') IS NOT NULL DROP PROCEDURE inventory.spPostGoodsReceipt;
EXEC ('
CREATE PROCEDURE inventory.spPostGoodsReceipt
    @TenantId INT,
    @GoodsReceiptHeaderId BIGINT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM inventory.GoodsReceiptHeaders
        WHERE GoodsReceiptHeaderId = @GoodsReceiptHeaderId
          AND TenantId = @TenantId
          AND Status IN (''DRAFT'', ''APPROVED'')
    )
        THROW 50002, ''GRN not found or not in postable status.'', 1;

    INSERT INTO inventory.StockLedger
    (
        TenantId, BranchId, LedgerDate, TransactionType, TransactionNo, ItemId, WarehouseId,
        BatchNo, ExpiryDate, QtyIn, QtyOut, UnitCost,
        ReferenceType, ReferenceId, CreatedUserId, CreatedDateTime
    )
    SELECT
        gh.TenantId,
        gh.BranchId,
        gh.GRNDate,
        ''GRN'',
        gh.GRNNo,
        gl.ItemId,
        gh.WarehouseId,
        gl.BatchNo,
        gl.ExpiryDate,
        gl.ReceivedQty,
        0,
        gl.UnitCost,
        ''GRN'',
        CAST(gh.GoodsReceiptHeaderId AS NVARCHAR(40)),
        @UserId,
        SYSDATETIME()
    FROM inventory.GoodsReceiptHeaders gh
    JOIN inventory.GoodsReceiptLines gl ON gl.GoodsReceiptHeaderId = gh.GoodsReceiptHeaderId
    WHERE gh.GoodsReceiptHeaderId = @GoodsReceiptHeaderId
      AND gh.TenantId = @TenantId;

    UPDATE inventory.GoodsReceiptHeaders
    SET Status = ''POSTED'',
        UpdatedUserId = @UserId,
        UpdatedDateTime = SYSDATETIME()
    WHERE GoodsReceiptHeaderId = @GoodsReceiptHeaderId
      AND TenantId = @TenantId;

    EXEC inventory.spRebuildStockBalances @TenantId = @TenantId, @UserId = @UserId;
END
');

IF OBJECT_ID('inventory.spPostStockTransfer', 'P') IS NOT NULL DROP PROCEDURE inventory.spPostStockTransfer;
EXEC ('
CREATE PROCEDURE inventory.spPostStockTransfer
    @TenantId INT,
    @StockTransferHeaderId BIGINT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TransferNo NVARCHAR(40), @TransferDate DATETIME2(0), @FromWarehouseId INT, @ToWarehouseId INT, @FromBranchId INT, @ToBranchId INT;

    SELECT
        @TransferNo = TransferNo,
        @TransferDate = TransferDate,
        @FromWarehouseId = FromWarehouseId,
        @ToWarehouseId = ToWarehouseId,
        @FromBranchId = FromBranchId,
        @ToBranchId = ToBranchId
    FROM inventory.StockTransferHeaders
    WHERE StockTransferHeaderId = @StockTransferHeaderId
      AND TenantId = @TenantId;

    IF @TransferNo IS NULL THROW 50003, ''Transfer not found.'', 1;

    INSERT INTO inventory.StockLedger
    (
        TenantId, BranchId, LedgerDate, TransactionType, TransactionNo, ItemId, WarehouseId,
        QtyIn, QtyOut, UnitCost, ReferenceType, ReferenceId, CreatedUserId, CreatedDateTime
    )
    SELECT
        @TenantId, @FromBranchId, @TransferDate, ''TRANSFER_OUT'', @TransferNo,
        tl.ItemId, @FromWarehouseId,
        0, tl.TransferQty, ISNULL(i.StandardCost, 0),
        ''TRANSFER'', CAST(@StockTransferHeaderId AS NVARCHAR(40)),
        @UserId, SYSDATETIME()
    FROM inventory.StockTransferLines tl
    JOIN inventory.Items i ON i.ItemId = tl.ItemId
    WHERE tl.StockTransferHeaderId = @StockTransferHeaderId
      AND tl.TenantId = @TenantId;

    INSERT INTO inventory.StockLedger
    (
        TenantId, BranchId, LedgerDate, TransactionType, TransactionNo, ItemId, WarehouseId,
        QtyIn, QtyOut, UnitCost, ReferenceType, ReferenceId, CreatedUserId, CreatedDateTime
    )
    SELECT
        @TenantId, @ToBranchId, @TransferDate, ''TRANSFER_IN'', @TransferNo,
        tl.ItemId, @ToWarehouseId,
        tl.TransferQty, 0, ISNULL(i.StandardCost, 0),
        ''TRANSFER'', CAST(@StockTransferHeaderId AS NVARCHAR(40)),
        @UserId, SYSDATETIME()
    FROM inventory.StockTransferLines tl
    JOIN inventory.Items i ON i.ItemId = tl.ItemId
    WHERE tl.StockTransferHeaderId = @StockTransferHeaderId
      AND tl.TenantId = @TenantId;

    UPDATE inventory.StockTransferHeaders
    SET Status = ''POSTED'',
        UpdatedUserId = @UserId,
        UpdatedDateTime = SYSDATETIME()
    WHERE StockTransferHeaderId = @StockTransferHeaderId
      AND TenantId = @TenantId;

    EXEC inventory.spRebuildStockBalances @TenantId = @TenantId, @UserId = @UserId;
END
');

/* =========================================================
   8) INDEXES
CREATE INDEX IX_inventory_Items_Tenant_NameArabic ON inventory.Items (TenantId, ItemNameArabic);
CREATE INDEX IX_inventory_Suppliers_Tenant_NameArabic ON inventory.Suppliers (TenantId, SupplierNameArabic);
CREATE INDEX IX_inventory_StockLedger_Tenant_Branch_ItemWarehouseDate ON inventory.StockLedger (TenantId, BranchId, ItemId, WarehouseId, LedgerDate);
CREATE INDEX IX_inventory_ItemStockBalances_Tenant_Branch_ItemWarehouse ON inventory.ItemStockBalances (TenantId, BranchId, ItemId, WarehouseId);
CREATE INDEX IX_inventory_StockReservations_Tenant_Branch_ItemWarehouse ON inventory.StockReservations (TenantId, BranchId, ItemId, WarehouseId);
CREATE INDEX IX_inventory_ItemBatches_Tenant_Expiry ON inventory.ItemBatches (TenantId, ExpiryDate);
CREATE INDEX IX_inventory_GoodsReceiptHeaders_Tenant_GRNDate ON inventory.GoodsReceiptHeaders (TenantId, GRNDate);

COMMIT TRANSACTION;
/*
    GCC-focused Inventory Module (SME Retail + Wholesale)
    ------------------------------------------------------
    Scope:
      - inventory schema creation
      - tenant-aware master + transaction tables
      - audit logging
      - views, scalar/table functions, stored procedures
      - stock ledger as source-of-truth + stock balance snapshot

    Notes:
      - All business tables contain: TenantId, CreatedUserId, CreatedDateTime, UpdatedUserId, UpdatedDateTime.
      - Arabic columns are included where market-facing fields are relevant.
      - GCC-specific coverage: VAT/Excise, batch/expiry tracking, wholesale tiers, regional currencies.
      - Branch master is reused from dbo.Branches (no inventory.Branches table is created).
      - If missing, IsWarehouse BIT NOT NULL DEFAULT(0) is added to dbo.Branches for warehouse designation.
*/

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'inventory')
    EXEC ('CREATE SCHEMA inventory');

/* =========================================================
   1) CORE CONFIG + AUDIT
IF OBJECT_ID('inventory.AuditLogs', 'U') IS NOT NULL DROP TABLE inventory.AuditLogs;
CREATE TABLE inventory.AuditLogs
(
    AuditLogId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ModuleName NVARCHAR(100) NOT NULL,
    EntityName NVARCHAR(120) NOT NULL,
    EntityId NVARCHAR(120) NULL,
    ActionType NVARCHAR(30) NOT NULL, -- INSERT/UPDATE/DELETE/POST/REVERSE
    OldData NVARCHAR(MAX) NULL,
    NewData NVARCHAR(MAX) NULL,
    IPAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(300) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_AuditLogs_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL
);

IF OBJECT_ID('inventory.Currencies', 'U') IS NOT NULL DROP TABLE inventory.Currencies;
CREATE TABLE inventory.Currencies
(
    CurrencyId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    CurrencyCode NVARCHAR(3) NOT NULL,
    CurrencyName NVARCHAR(60) NOT NULL,
    CurrencyNameArabic NVARCHAR(60) NULL,
    Symbol NVARCHAR(10) NULL,
    IsBaseCurrency BIT NOT NULL CONSTRAINT DF_inventory_Currencies_IsBaseCurrency DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Currencies_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Currencies_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Currencies UNIQUE (TenantId, CurrencyCode)
);

IF OBJECT_ID('inventory.ExchangeRates', 'U') IS NOT NULL DROP TABLE inventory.ExchangeRates;
CREATE TABLE inventory.ExchangeRates
(
    ExchangeRateId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    CurrencyId INT NOT NULL,
    RateDate DATE NOT NULL,
    BuyRate DECIMAL(18,6) NOT NULL,
    SellRate DECIMAL(18,6) NOT NULL,
    AverageRate DECIMAL(18,6) NOT NULL,
    SourceName NVARCHAR(100) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ExchangeRates_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ExchangeRates UNIQUE (TenantId, CurrencyId, RateDate),
    CONSTRAINT FK_inventory_ExchangeRates_Currency FOREIGN KEY (CurrencyId) REFERENCES inventory.Currencies(CurrencyId)
);

IF OBJECT_ID('inventory.TaxCodes', 'U') IS NOT NULL DROP TABLE inventory.TaxCodes;
CREATE TABLE inventory.TaxCodes
(
    TaxCodeId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    TaxCode NVARCHAR(20) NOT NULL,
    TaxName NVARCHAR(100) NOT NULL,
    TaxNameArabic NVARCHAR(100) NULL,
    TaxType NVARCHAR(20) NOT NULL, -- VAT/EXCISE/ZERO/EXEMPT
    TaxRate DECIMAL(5,2) NOT NULL,
    IsInclusive BIT NOT NULL CONSTRAINT DF_inventory_TaxCodes_IsInclusive DEFAULT (0),
    CountryCode NVARCHAR(3) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_TaxCodes_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_TaxCodes_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_TaxCodes UNIQUE (TenantId, TaxCode)
);

/* =========================================================
   2) MASTER DATA
IF OBJECT_ID('inventory.Suppliers', 'U') IS NOT NULL DROP TABLE inventory.Suppliers;
CREATE TABLE inventory.Suppliers
(
    SupplierId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    SupplierCode NVARCHAR(30) NOT NULL,
    SupplierName NVARCHAR(200) NOT NULL,
    SupplierNameArabic NVARCHAR(200) NULL,
    ContactPerson NVARCHAR(120) NULL,
    ContactPersonArabic NVARCHAR(120) NULL,
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
    IsApprovedVendor BIT NOT NULL CONSTRAINT DF_inventory_Suppliers_IsApprovedVendor DEFAULT (1),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Suppliers_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Suppliers_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Suppliers UNIQUE (TenantId, SupplierCode)
);

IF OBJECT_ID('inventory.Units', 'U') IS NOT NULL DROP TABLE inventory.Units;
CREATE TABLE inventory.Units
(
    UnitId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    UnitCode NVARCHAR(20) NOT NULL,
    UnitName NVARCHAR(100) NOT NULL,
    UnitNameArabic NVARCHAR(100) NULL,
    DecimalPrecision TINYINT NOT NULL CONSTRAINT DF_inventory_Units_DecimalPrecision DEFAULT (3),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Units_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Units_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Units UNIQUE (TenantId, UnitCode)
);

IF OBJECT_ID('inventory.ItemCategories', 'U') IS NOT NULL DROP TABLE inventory.ItemCategories;
CREATE TABLE inventory.ItemCategories
(
    ItemCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    CategoryCode NVARCHAR(30) NOT NULL,
    CategoryName NVARCHAR(150) NOT NULL,
    CategoryNameArabic NVARCHAR(150) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemCategories_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemCategories_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemCategories UNIQUE (TenantId, CategoryCode)
);

IF OBJECT_ID('inventory.ItemSubCategories', 'U') IS NOT NULL DROP TABLE inventory.ItemSubCategories;
CREATE TABLE inventory.ItemSubCategories
(
    ItemSubCategoryId INT IDENTITY(1,1) PRIMARY KEY,
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
    CONSTRAINT UQ_inventory_ItemSubCategories UNIQUE (TenantId, ItemCategoryId, SubCategoryCode),
    CONSTRAINT FK_inventory_ItemSubCategories_Category FOREIGN KEY (ItemCategoryId) REFERENCES inventory.ItemCategories(ItemCategoryId)
);

IF OBJECT_ID('inventory.Brands', 'U') IS NOT NULL DROP TABLE inventory.Brands;
CREATE TABLE inventory.Brands
(
    BrandId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BrandCode NVARCHAR(30) NOT NULL,
    BrandName NVARCHAR(120) NOT NULL,
    BrandNameArabic NVARCHAR(120) NULL,
    CountryCode NVARCHAR(3) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Brands_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Brands_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Brands UNIQUE (TenantId, BrandCode)
);

IF OBJECT_ID('inventory.Items', 'U') IS NOT NULL DROP TABLE inventory.Items;
CREATE TABLE inventory.Items
(
    ItemId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemCode NVARCHAR(40) NOT NULL,
    SKU NVARCHAR(40) NULL,
    Barcode NVARCHAR(50) NULL,
    ItemName NVARCHAR(200) NOT NULL,
    ItemNameArabic NVARCHAR(200) NULL,
    ItemShortName NVARCHAR(100) NULL,
    ItemShortNameArabic NVARCHAR(100) NULL,
    Description NVARCHAR(1000) NULL,
    DescriptionArabic NVARCHAR(1000) NULL,
    BrandId INT NULL,
    ItemCategoryId INT NOT NULL,
    ItemSubCategoryId INT NULL,
    BaseUnitId INT NOT NULL,
    SalesUnitId INT NULL,
    PurchaseUnitId INT NULL,
    TaxCodeId INT NULL,
    PreferredSupplierId BIGINT NULL,
    OriginCountryCode NVARCHAR(3) NULL,
    HSCode NVARCHAR(20) NULL,
    IsBatchTracked BIT NOT NULL CONSTRAINT DF_inventory_Items_IsBatchTracked DEFAULT (0),
    IsSerialTracked BIT NOT NULL CONSTRAINT DF_inventory_Items_IsSerialTracked DEFAULT (0),
    IsExpiryTracked BIT NOT NULL CONSTRAINT DF_inventory_Items_IsExpiryTracked DEFAULT (0),
    ShelfLifeDays INT NULL,
    MinSellingPrice DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_Items_MinSellingPrice DEFAULT (0),
    StandardCost DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_Items_StandardCost DEFAULT (0),
    IsHalalCertified BIT NOT NULL CONSTRAINT DF_inventory_Items_IsHalalCertified DEFAULT (0), -- GCC unique
    RamadanDemandFactor DECIMAL(6,3) NOT NULL CONSTRAINT DF_inventory_Items_RamadanDemandFactor DEFAULT (1.000), -- unique demand tuning
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Items_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Items_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Items UNIQUE (TenantId, ItemCode),
    CONSTRAINT FK_inventory_Items_Brand FOREIGN KEY (BrandId) REFERENCES inventory.Brands(BrandId),
    CONSTRAINT FK_inventory_Items_Category FOREIGN KEY (ItemCategoryId) REFERENCES inventory.ItemCategories(ItemCategoryId),
    CONSTRAINT FK_inventory_Items_SubCategory FOREIGN KEY (ItemSubCategoryId) REFERENCES inventory.ItemSubCategories(ItemSubCategoryId),
    CONSTRAINT FK_inventory_Items_BaseUnit FOREIGN KEY (BaseUnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_Items_SalesUnit FOREIGN KEY (SalesUnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_Items_PurchaseUnit FOREIGN KEY (PurchaseUnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_Items_TaxCode FOREIGN KEY (TaxCodeId) REFERENCES inventory.TaxCodes(TaxCodeId),
    CONSTRAINT FK_inventory_Items_Supplier FOREIGN KEY (PreferredSupplierId) REFERENCES inventory.Suppliers(SupplierId)
);

IF OBJECT_ID('inventory.ItemBarcodes', 'U') IS NOT NULL DROP TABLE inventory.ItemBarcodes;
CREATE TABLE inventory.ItemBarcodes
(
    ItemBarcodeId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    Barcode NVARCHAR(50) NOT NULL,
    IsPrimary BIT NOT NULL CONSTRAINT DF_inventory_ItemBarcodes_IsPrimary DEFAULT (0),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemBarcodes_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemBarcodes UNIQUE (TenantId, Barcode),
    CONSTRAINT FK_inventory_ItemBarcodes_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemBarcodes_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId)
);

IF OBJECT_ID('inventory.ItemSuppliers', 'U') IS NOT NULL DROP TABLE inventory.ItemSuppliers;
CREATE TABLE inventory.ItemSuppliers
(
    ItemSupplierId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    SupplierId BIGINT NOT NULL,
    SupplierItemCode NVARCHAR(50) NULL,
    LastPurchasePrice DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_ItemSuppliers_LastPurchasePrice DEFAULT (0),
    LeadTimeDays INT NOT NULL CONSTRAINT DF_inventory_ItemSuppliers_LeadTimeDays DEFAULT (0),
    IsPreferred BIT NOT NULL CONSTRAINT DF_inventory_ItemSuppliers_IsPreferred DEFAULT (0),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemSuppliers_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemSuppliers UNIQUE (TenantId, ItemId, SupplierId),
    CONSTRAINT FK_inventory_ItemSuppliers_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemSuppliers_Supplier FOREIGN KEY (SupplierId) REFERENCES inventory.Suppliers(SupplierId)
);

IF OBJECT_ID('inventory.ItemUnitConversions', 'U') IS NOT NULL DROP TABLE inventory.ItemUnitConversions;
CREATE TABLE inventory.ItemUnitConversions
(
    ItemUnitConversionId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    FromUnitId INT NOT NULL,
    ToUnitId INT NOT NULL,
    ConversionFactor DECIMAL(18,6) NOT NULL,
    RoundingMode NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_ItemUnitConversions_RoundingMode DEFAULT ('ROUND'),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemUnitConversions_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemUnitConversions_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemUnitConversions UNIQUE (TenantId, ItemId, FromUnitId, ToUnitId),
    CONSTRAINT FK_inventory_ItemUnitConversions_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemUnitConversions_From FOREIGN KEY (FromUnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_ItemUnitConversions_To FOREIGN KEY (ToUnitId) REFERENCES inventory.Units(UnitId)
);

IF OBJECT_ID('dbo.Branches', 'U') IS NULL
    THROW 50001, 'dbo.Branches table is required for inventory script.', 1;

IF COL_LENGTH('dbo.Branches', 'IsWarehouse') IS NULL
BEGIN
    ALTER TABLE dbo.Branches
    ADD IsWarehouse BIT NOT NULL
        CONSTRAINT DF_dbo_Branches_IsWarehouse DEFAULT (0);
END;

IF OBJECT_ID('inventory.Warehouses', 'U') IS NOT NULL DROP TABLE inventory.Warehouses;
CREATE TABLE inventory.Warehouses
(
    WarehouseId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    WarehouseCode NVARCHAR(30) NOT NULL,
    WarehouseName NVARCHAR(150) NOT NULL,
    WarehouseNameArabic NVARCHAR(150) NULL,
    Address NVARCHAR(300) NULL,
    AddressArabic NVARCHAR(300) NULL,
    City NVARCHAR(100) NULL,
    CityArabic NVARCHAR(100) NULL,
    TemperatureZone NVARCHAR(30) NULL,
    IsTransitWarehouse BIT NOT NULL CONSTRAINT DF_inventory_Warehouses_IsTransitWarehouse DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_Warehouses_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_Warehouses_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_Warehouses UNIQUE (TenantId, BranchId, WarehouseCode),
    CONSTRAINT FK_inventory_Warehouses_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.WarehouseBins', 'U') IS NOT NULL DROP TABLE inventory.WarehouseBins;
CREATE TABLE inventory.WarehouseBins
(
    WarehouseBinId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    WarehouseId INT NOT NULL,
    BinCode NVARCHAR(40) NOT NULL,
    BinName NVARCHAR(120) NOT NULL,
    BinNameArabic NVARCHAR(120) NULL,
    IsPickFace BIT NOT NULL CONSTRAINT DF_inventory_WarehouseBins_IsPickFace DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_WarehouseBins_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_WarehouseBins_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_WarehouseBins UNIQUE (TenantId, WarehouseId, BinCode),
    CONSTRAINT FK_inventory_WarehouseBins_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId)
);

IF OBJECT_ID('inventory.PriceLists', 'U') IS NOT NULL DROP TABLE inventory.PriceLists;
CREATE TABLE inventory.PriceLists
(
    PriceListId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    PriceListCode NVARCHAR(30) NOT NULL,
    PriceListName NVARCHAR(150) NOT NULL,
    PriceListNameArabic NVARCHAR(150) NULL,
    PriceListType NVARCHAR(20) NOT NULL, -- RETAIL/WHOLESALE/ONLINE/PROMO
    CurrencyId INT NOT NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_PriceLists_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_PriceLists_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_PriceLists UNIQUE (TenantId, PriceListCode),
    CONSTRAINT FK_inventory_PriceLists_Currency FOREIGN KEY (CurrencyId) REFERENCES inventory.Currencies(CurrencyId)
);

IF OBJECT_ID('inventory.ItemPrices', 'U') IS NOT NULL DROP TABLE inventory.ItemPrices;
CREATE TABLE inventory.ItemPrices
(
    ItemPriceId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    PriceListId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    MinQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_ItemPrices_MinQty DEFAULT (1),
    Price DECIMAL(18,3) NOT NULL,
    DiscountPercent DECIMAL(5,2) NOT NULL CONSTRAINT DF_inventory_ItemPrices_DiscountPercent DEFAULT (0),
    TaxCodeId INT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_inventory_ItemPrices_IsActive DEFAULT (1),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemPrices_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemPrices UNIQUE (TenantId, PriceListId, ItemId, UnitId, MinQty),
    CONSTRAINT FK_inventory_ItemPrices_PriceList FOREIGN KEY (PriceListId) REFERENCES inventory.PriceLists(PriceListId),
    CONSTRAINT FK_inventory_ItemPrices_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemPrices_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId),
    CONSTRAINT FK_inventory_ItemPrices_TaxCode FOREIGN KEY (TaxCodeId) REFERENCES inventory.TaxCodes(TaxCodeId)
);

IF OBJECT_ID('inventory.ReorderPolicies', 'U') IS NOT NULL DROP TABLE inventory.ReorderPolicies;
CREATE TABLE inventory.ReorderPolicies
(
    ReorderPolicyId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    MinStock DECIMAL(18,6) NOT NULL,
    MaxStock DECIMAL(18,6) NOT NULL,
    ReorderQty DECIMAL(18,6) NOT NULL,
    SafetyDays INT NOT NULL CONSTRAINT DF_inventory_ReorderPolicies_SafetyDays DEFAULT (7),
    SeasonalityMultiplier DECIMAL(6,3) NOT NULL CONSTRAINT DF_inventory_ReorderPolicies_SeasonalityMultiplier DEFAULT (1.000),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ReorderPolicies_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ReorderPolicies UNIQUE (TenantId, BranchId, ItemId, WarehouseId),
    CONSTRAINT FK_inventory_ReorderPolicies_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ReorderPolicies_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_ReorderPolicies_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

/* =========================================================
   3) STOCK + MOVEMENT TABLES
IF OBJECT_ID('inventory.StockLedger', 'U') IS NOT NULL DROP TABLE inventory.StockLedger;
CREATE TABLE inventory.StockLedger
(
    StockLedgerId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    LedgerDate DATETIME2(0) NOT NULL,
    TransactionType NVARCHAR(30) NOT NULL,
    TransactionNo NVARCHAR(40) NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    WarehouseBinId BIGINT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    SerialNo NVARCHAR(100) NULL,
    QtyIn DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_StockLedger_QtyIn DEFAULT (0),
    QtyOut DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_StockLedger_QtyOut DEFAULT (0),
    UnitCost DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_StockLedger_UnitCost DEFAULT (0),
    ValueImpact AS ((QtyIn - QtyOut) * UnitCost) PERSISTED,
    ReferenceType NVARCHAR(30) NULL,
    ReferenceId NVARCHAR(40) NULL,
    Notes NVARCHAR(500) NULL,
    NotesArabic NVARCHAR(500) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockLedger_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_StockLedger_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_StockLedger_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockLedger_Bin FOREIGN KEY (WarehouseBinId) REFERENCES inventory.WarehouseBins(WarehouseBinId),
    CONSTRAINT FK_inventory_StockLedger_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.ItemStockBalances', 'U') IS NOT NULL DROP TABLE inventory.ItemStockBalances;
CREATE TABLE inventory.ItemStockBalances
(
    ItemStockBalanceId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    WarehouseBinId BIGINT NULL,
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
    CONSTRAINT UQ_inventory_ItemStockBalances UNIQUE (TenantId, BranchId, ItemId, WarehouseId, WarehouseBinId, BatchNo, ExpiryDate),
    CONSTRAINT FK_inventory_ItemStockBalances_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemStockBalances_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_ItemStockBalances_Bin FOREIGN KEY (WarehouseBinId) REFERENCES inventory.WarehouseBins(WarehouseBinId),
    CONSTRAINT FK_inventory_ItemStockBalances_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.StockReservations', 'U') IS NOT NULL DROP TABLE inventory.StockReservations;
CREATE TABLE inventory.StockReservations
(
    StockReservationId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ReservationNo NVARCHAR(40) NOT NULL,
    ReservationType NVARCHAR(20) NOT NULL, -- SALES_ORDER/TRANSFER/PICKLIST
    ReservationDate DATETIME2(0) NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ReservedQty DECIMAL(18,6) NOT NULL,
    ReleasedQty DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_StockReservations_ReleasedQty DEFAULT (0),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_StockReservations_Status DEFAULT ('OPEN'),
    Remarks NVARCHAR(500) NULL,
    RemarksArabic NVARCHAR(500) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockReservations_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_StockReservations UNIQUE (TenantId, ReservationNo),
    CONSTRAINT FK_inventory_StockReservations_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_StockReservations_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockReservations_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.ItemBatches', 'U') IS NOT NULL DROP TABLE inventory.ItemBatches;
CREATE TABLE inventory.ItemBatches
(
    ItemBatchId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    BatchNo NVARCHAR(60) NOT NULL,
    MfgDate DATE NULL,
    ExpiryDate DATE NULL,
    CountryOfOrigin NVARCHAR(3) NULL,
    SupplierId BIGINT NULL,
    QtyOnHand DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_ItemBatches_QtyOnHand DEFAULT (0),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_ItemBatches_Status DEFAULT ('ACTIVE'),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemBatches_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemBatches UNIQUE (TenantId, ItemId, WarehouseId, BatchNo),
    CONSTRAINT FK_inventory_ItemBatches_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemBatches_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_ItemBatches_Supplier FOREIGN KEY (SupplierId) REFERENCES inventory.Suppliers(SupplierId),
    CONSTRAINT FK_inventory_ItemBatches_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

IF OBJECT_ID('inventory.ItemSerials', 'U') IS NOT NULL DROP TABLE inventory.ItemSerials;
CREATE TABLE inventory.ItemSerials
(
    ItemSerialId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    ItemId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    SerialNo NVARCHAR(100) NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    WarrantyStartDate DATE NULL,
    WarrantyEndDate DATE NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_ItemSerials_Status DEFAULT ('IN_STOCK'),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_ItemSerials_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_ItemSerials UNIQUE (TenantId, SerialNo),
    CONSTRAINT FK_inventory_ItemSerials_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_ItemSerials_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_ItemSerials_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);

/* =========================================================
   4) PROCUREMENT + INTERNAL MOVEMENT
IF OBJECT_ID('inventory.GoodsReceiptHeaders', 'U') IS NOT NULL DROP TABLE inventory.GoodsReceiptHeaders;
IF OBJECT_ID('inventory.GoodsReceiptLines', 'U') IS NOT NULL DROP TABLE inventory.GoodsReceiptLines;
CREATE TABLE inventory.GoodsReceiptHeaders
(
    GoodsReceiptHeaderId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    GRNNo NVARCHAR(40) NOT NULL,
    GRNDate DATETIME2(0) NOT NULL,
    SupplierId BIGINT NOT NULL,
    WarehouseId INT NOT NULL,
    CurrencyId INT NOT NULL,
    ExchangeRate DECIMAL(18,6) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_ExchangeRate DEFAULT (1),
    SupplierInvoiceNo NVARCHAR(60) NULL,
    SupplierInvoiceDate DATE NULL,
    SubTotal DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_SubTotal DEFAULT (0),
    DiscountAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_DiscountAmount DEFAULT (0),
    VATAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_VATAmount DEFAULT (0),
    ExciseAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_ExciseAmount DEFAULT (0),
    NetTotal DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_NetTotal DEFAULT (0),
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_Status DEFAULT ('DRAFT'),
    Remarks NVARCHAR(500) NULL,
    RemarksArabic NVARCHAR(500) NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptHeaders_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_GoodsReceiptHeaders UNIQUE (TenantId, GRNNo),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Supplier FOREIGN KEY (SupplierId) REFERENCES inventory.Suppliers(SupplierId),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Currency FOREIGN KEY (CurrencyId) REFERENCES inventory.Currencies(CurrencyId),
    CONSTRAINT FK_inventory_GoodsReceiptHeaders_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);
CREATE TABLE inventory.GoodsReceiptLines
(
    GoodsReceiptLineId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    GoodsReceiptHeaderId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    ReceivedQty DECIMAL(18,6) NOT NULL,
    UnitCost DECIMAL(18,3) NOT NULL,
    DiscountPercent DECIMAL(5,2) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_DiscountPercent DEFAULT (0),
    VATAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_VATAmount DEFAULT (0),
    ExciseAmount DECIMAL(18,3) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_ExciseAmount DEFAULT (0),
    LineTotal DECIMAL(18,3) NOT NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_GoodsReceiptLines_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_GoodsReceiptLines_Header FOREIGN KEY (GoodsReceiptHeaderId) REFERENCES inventory.GoodsReceiptHeaders(GoodsReceiptHeaderId),
    CONSTRAINT FK_inventory_GoodsReceiptLines_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_GoodsReceiptLines_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId)
);

IF OBJECT_ID('inventory.StockTransferHeaders', 'U') IS NOT NULL DROP TABLE inventory.StockTransferHeaders;
IF OBJECT_ID('inventory.StockTransferLines', 'U') IS NOT NULL DROP TABLE inventory.StockTransferLines;
CREATE TABLE inventory.StockTransferHeaders
(
    StockTransferHeaderId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    FromBranchId INT NOT NULL,
    ToBranchId INT NOT NULL,
    TransferNo NVARCHAR(40) NOT NULL,
    TransferDate DATETIME2(0) NOT NULL,
    FromWarehouseId INT NOT NULL,
    ToWarehouseId INT NOT NULL,
    TransferReason NVARCHAR(200) NULL,
    TransferReasonArabic NVARCHAR(200) NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_StockTransferHeaders_Status DEFAULT ('DRAFT'),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockTransferHeaders_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_StockTransferHeaders UNIQUE (TenantId, TransferNo),
    CONSTRAINT FK_inventory_StockTransferHeaders_FromWarehouse FOREIGN KEY (FromWarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockTransferHeaders_ToWarehouse FOREIGN KEY (ToWarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockTransferHeaders_FromBranch FOREIGN KEY (FromBranchId) REFERENCES dbo.Branches(Id),
    CONSTRAINT FK_inventory_StockTransferHeaders_ToBranch FOREIGN KEY (ToBranchId) REFERENCES dbo.Branches(Id)
);
CREATE TABLE inventory.StockTransferLines
(
    StockTransferLineId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    StockTransferHeaderId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    TransferQty DECIMAL(18,6) NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockTransferLines_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_StockTransferLines_Header FOREIGN KEY (StockTransferHeaderId) REFERENCES inventory.StockTransferHeaders(StockTransferHeaderId),
    CONSTRAINT FK_inventory_StockTransferLines_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_StockTransferLines_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId)
);

IF OBJECT_ID('inventory.StockAdjustmentHeaders', 'U') IS NOT NULL DROP TABLE inventory.StockAdjustmentHeaders;
IF OBJECT_ID('inventory.StockAdjustmentLines', 'U') IS NOT NULL DROP TABLE inventory.StockAdjustmentLines;
CREATE TABLE inventory.StockAdjustmentHeaders
(
    StockAdjustmentHeaderId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    BranchId INT NOT NULL,
    AdjustmentNo NVARCHAR(40) NOT NULL,
    AdjustmentDate DATETIME2(0) NOT NULL,
    WarehouseId INT NOT NULL,
    AdjustmentReason NVARCHAR(200) NOT NULL,
    AdjustmentReasonArabic NVARCHAR(200) NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_inventory_StockAdjustmentHeaders_Status DEFAULT ('DRAFT'),
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockAdjustmentHeaders_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT UQ_inventory_StockAdjustmentHeaders UNIQUE (TenantId, AdjustmentNo),
    CONSTRAINT FK_inventory_StockAdjustmentHeaders_Warehouse FOREIGN KEY (WarehouseId) REFERENCES inventory.Warehouses(WarehouseId),
    CONSTRAINT FK_inventory_StockAdjustmentHeaders_Branch FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id)
);
CREATE TABLE inventory.StockAdjustmentLines
(
    StockAdjustmentLineId BIGINT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    StockAdjustmentHeaderId BIGINT NOT NULL,
    ItemId BIGINT NOT NULL,
    UnitId INT NOT NULL,
    SystemQty DECIMAL(18,6) NOT NULL,
    CountedQty DECIMAL(18,6) NOT NULL,
    DifferenceQty AS (CountedQty - SystemQty) PERSISTED,
    UnitCost DECIMAL(18,3) NOT NULL,
    BatchNo NVARCHAR(60) NULL,
    ExpiryDate DATE NULL,
    CreatedUserId BIGINT NOT NULL,
    CreatedDateTime DATETIME2(0) NOT NULL CONSTRAINT DF_inventory_StockAdjustmentLines_CreatedDateTime DEFAULT (SYSDATETIME()),
    UpdatedUserId BIGINT NULL,
    UpdatedDateTime DATETIME2(0) NULL,
    CONSTRAINT FK_inventory_StockAdjustmentLines_Header FOREIGN KEY (StockAdjustmentHeaderId) REFERENCES inventory.StockAdjustmentHeaders(StockAdjustmentHeaderId),
    CONSTRAINT FK_inventory_StockAdjustmentLines_Item FOREIGN KEY (ItemId) REFERENCES inventory.Items(ItemId),
    CONSTRAINT FK_inventory_StockAdjustmentLines_Unit FOREIGN KEY (UnitId) REFERENCES inventory.Units(UnitId)
);

/* =========================================================
   5) VIEWS
IF OBJECT_ID('inventory.vwCurrentStock', 'V') IS NOT NULL DROP VIEW inventory.vwCurrentStock;
EXEC ('
CREATE VIEW inventory.vwCurrentStock
AS
SELECT
    sb.TenantId,
    sb.ItemId,
    i.ItemCode,
    i.ItemName,
    i.ItemNameArabic,
    sb.BranchId,
    sb.WarehouseId,
    w.WarehouseCode,
    w.WarehouseName,
    sb.BatchNo,
    sb.ExpiryDate,
    sb.OnHandQty,
    sb.ReservedQty,
    sb.AvailableQty,
    sb.AvgCost,
    (sb.OnHandQty * sb.AvgCost) AS StockValue
FROM inventory.ItemStockBalances sb
JOIN inventory.Items i ON i.ItemId = sb.ItemId
JOIN inventory.Warehouses w ON w.WarehouseId = sb.WarehouseId
');

IF OBJECT_ID('inventory.vwItemStockRuntime', 'V') IS NOT NULL DROP VIEW inventory.vwItemStockRuntime;
EXEC ('
CREATE VIEW inventory.vwItemStockRuntime
AS
SELECT
    sl.TenantId,
    sl.ItemId,
    sl.BranchId,
    sl.WarehouseId,
    sl.BatchNo,
    sl.ExpiryDate,
    SUM(sl.QtyIn - sl.QtyOut) AS RuntimeOnHandQty,
    MAX(sl.LedgerDate) AS LastMovementDate
FROM inventory.StockLedger sl
GROUP BY sl.TenantId, sl.BranchId, sl.ItemId, sl.WarehouseId, sl.BatchNo, sl.ExpiryDate
');

IF OBJECT_ID('inventory.vwNearExpiryStock', 'V') IS NOT NULL DROP VIEW inventory.vwNearExpiryStock;
EXEC ('
CREATE VIEW inventory.vwNearExpiryStock
AS
SELECT
    sb.TenantId,
    sb.ItemId,
    i.ItemCode,
    i.ItemName,
    sb.BranchId,
    sb.WarehouseId,
    sb.BatchNo,
    sb.ExpiryDate,
    DATEDIFF(DAY, CAST(GETDATE() AS DATE), sb.ExpiryDate) AS DaysToExpiry,
    sb.OnHandQty
FROM inventory.ItemStockBalances sb
JOIN inventory.Items i ON i.ItemId = sb.ItemId
WHERE sb.ExpiryDate IS NOT NULL AND sb.OnHandQty > 0
');

IF OBJECT_ID('inventory.vwReorderSuggestions', 'V') IS NOT NULL DROP VIEW inventory.vwReorderSuggestions;
EXEC ('
CREATE VIEW inventory.vwReorderSuggestions
AS
SELECT
    rp.TenantId,
    rp.ItemId,
    i.ItemCode,
    i.ItemName,
    rp.WarehouseId,
    rp.MinStock,
    rp.MaxStock,
    ISNULL(cs.OnHandQty, 0) AS CurrentStock,
    CASE WHEN ISNULL(cs.OnHandQty, 0) < rp.MinStock THEN (rp.MaxStock - ISNULL(cs.OnHandQty, 0)) ELSE 0 END AS SuggestedOrderQty
FROM inventory.ReorderPolicies rp
JOIN inventory.Items i ON i.ItemId = rp.ItemId
OUTER APPLY
(
    SELECT SUM(sb.OnHandQty) AS OnHandQty
    FROM inventory.ItemStockBalances sb
    WHERE sb.TenantId = rp.TenantId
      AND sb.ItemId = rp.ItemId
      AND sb.BranchId = rp.BranchId
      AND sb.WarehouseId = rp.WarehouseId
) cs
');

/* =========================================================
   6) FUNCTIONS
IF OBJECT_ID('inventory.fnStockOnHand', 'FN') IS NOT NULL DROP FUNCTION inventory.fnStockOnHand;
EXEC ('
CREATE FUNCTION inventory.fnStockOnHand
(
    @TenantId INT,
    @ItemId BIGINT,
    @BranchId INT,
    @WarehouseId INT
)
RETURNS DECIMAL(18,6)
AS
BEGIN
    DECLARE @Qty DECIMAL(18,6);

    SELECT @Qty = ISNULL(SUM(sb.OnHandQty), 0)
    FROM inventory.ItemStockBalances sb
    WHERE sb.TenantId = @TenantId
      AND sb.ItemId = @ItemId
      AND sb.BranchId = @BranchId
      AND sb.WarehouseId = @WarehouseId;

    RETURN ISNULL(@Qty, 0);
END
');

IF OBJECT_ID('inventory.fnAvailableStock', 'FN') IS NOT NULL DROP FUNCTION inventory.fnAvailableStock;
EXEC ('
CREATE FUNCTION inventory.fnAvailableStock
(
    @TenantId INT,
    @ItemId BIGINT,
    @BranchId INT,
    @WarehouseId INT
)
RETURNS DECIMAL(18,6)
AS
BEGIN
    DECLARE @Qty DECIMAL(18,6);

    SELECT @Qty = ISNULL(SUM(sb.AvailableQty), 0)
    FROM inventory.ItemStockBalances sb
    WHERE sb.TenantId = @TenantId
      AND sb.ItemId = @ItemId
      AND sb.BranchId = @BranchId
      AND sb.WarehouseId = @WarehouseId;

    RETURN ISNULL(@Qty, 0);
END
');

/* =========================================================
   7) PROCEDURES
IF OBJECT_ID('inventory.spCreateAuditLog', 'P') IS NOT NULL DROP PROCEDURE inventory.spCreateAuditLog;
EXEC ('
CREATE PROCEDURE inventory.spCreateAuditLog
    @TenantId INT,
    @ModuleName NVARCHAR(100),
    @EntityName NVARCHAR(120),
    @EntityId NVARCHAR(120) = NULL,
    @ActionType NVARCHAR(30),
    @OldData NVARCHAR(MAX) = NULL,
    @NewData NVARCHAR(MAX) = NULL,
    @CreatedUserId BIGINT,
    @IPAddress NVARCHAR(45) = NULL,
    @UserAgent NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO inventory.AuditLogs
    (
        TenantId, ModuleName, EntityName, EntityId, ActionType,
        OldData, NewData, IPAddress, UserAgent,
        CreatedUserId, CreatedDateTime
    )
    VALUES
    (
        @TenantId, @ModuleName, @EntityName, @EntityId, @ActionType,
        @OldData, @NewData, @IPAddress, @UserAgent,
        @CreatedUserId, SYSDATETIME()
    );
END
');

IF OBJECT_ID('inventory.spRebuildStockBalances', 'P') IS NOT NULL DROP PROCEDURE inventory.spRebuildStockBalances;
EXEC ('
CREATE PROCEDURE inventory.spRebuildStockBalances
    @TenantId INT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM inventory.ItemStockBalances WHERE TenantId = @TenantId;

    INSERT INTO inventory.ItemStockBalances
    (
        TenantId, BranchId, ItemId, WarehouseId, WarehouseBinId, BatchNo, ExpiryDate,
        OnHandQty, ReservedQty, AvgCost, LastTransactionDate,
        CreatedUserId, CreatedDateTime
    )
    SELECT
        sl.TenantId,
        sl.BranchId,
        sl.ItemId,
        sl.WarehouseId,
        sl.WarehouseBinId,
        sl.BatchNo,
        sl.ExpiryDate,
        SUM(sl.QtyIn - sl.QtyOut) AS OnHandQty,
        0 AS ReservedQty,
        CASE WHEN SUM(sl.QtyIn - sl.QtyOut) = 0 THEN 0
             ELSE SUM((sl.QtyIn - sl.QtyOut) * sl.UnitCost) / NULLIF(SUM(sl.QtyIn - sl.QtyOut),0)
        END AS AvgCost,
        MAX(sl.LedgerDate) AS LastTransactionDate,
        @UserId,
        SYSDATETIME()
    FROM inventory.StockLedger sl
    WHERE sl.TenantId = @TenantId
    GROUP BY sl.TenantId, sl.BranchId, sl.ItemId, sl.WarehouseId, sl.WarehouseBinId, sl.BatchNo, sl.ExpiryDate;

    EXEC inventory.spCreateAuditLog
        @TenantId = @TenantId,
        @ModuleName = ''Inventory'',
        @EntityName = ''ItemStockBalances'',
        @ActionType = ''REBUILD'',
        @NewData = ''Stock balances rebuilt from ledger'',
        @CreatedUserId = @UserId;
END
');

IF OBJECT_ID('inventory.spReserveStock', 'P') IS NOT NULL DROP PROCEDURE inventory.spReserveStock;
EXEC ('
CREATE PROCEDURE inventory.spReserveStock
    @TenantId INT,
    @ReservationNo NVARCHAR(40),
    @ReservationType NVARCHAR(20),
    @ItemId BIGINT,
    @BranchId INT,
    @WarehouseId INT,
    @ReservedQty DECIMAL(18,6),
    @UserId BIGINT,
    @Remarks NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Available DECIMAL(18,6) = inventory.fnAvailableStock(@TenantId, @ItemId, @BranchId, @WarehouseId);

    IF @Available < @ReservedQty
        THROW 50001, ''Insufficient available stock for reservation.'', 1;

    INSERT INTO inventory.StockReservations
    (
        TenantId, ReservationNo, ReservationType, ReservationDate, ItemId, BranchId, WarehouseId,
        ReservedQty, Remarks, CreatedUserId, CreatedDateTime
    )
    VALUES
    (
        @TenantId, @ReservationNo, @ReservationType, SYSDATETIME(), @ItemId, @BranchId, @WarehouseId,
        @ReservedQty, @Remarks, @UserId, SYSDATETIME()
    );

    UPDATE sb
    SET sb.ReservedQty = sb.ReservedQty + @ReservedQty,
        sb.UpdatedUserId = @UserId,
        sb.UpdatedDateTime = SYSDATETIME()
    FROM inventory.ItemStockBalances sb
    WHERE sb.TenantId = @TenantId
      AND sb.ItemId = @ItemId
      AND sb.BranchId = @BranchId
      AND sb.WarehouseId = @WarehouseId;
END
');

IF OBJECT_ID('inventory.spPostGoodsReceipt', 'P') IS NOT NULL DROP PROCEDURE inventory.spPostGoodsReceipt;
EXEC ('
CREATE PROCEDURE inventory.spPostGoodsReceipt
    @TenantId INT,
    @GoodsReceiptHeaderId BIGINT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM inventory.GoodsReceiptHeaders
        WHERE GoodsReceiptHeaderId = @GoodsReceiptHeaderId
          AND TenantId = @TenantId
          AND Status IN (''DRAFT'', ''APPROVED'')
    )
        THROW 50002, ''GRN not found or not in postable status.'', 1;

    INSERT INTO inventory.StockLedger
    (
        TenantId, BranchId, LedgerDate, TransactionType, TransactionNo, ItemId, WarehouseId,
        BatchNo, ExpiryDate, QtyIn, QtyOut, UnitCost,
        ReferenceType, ReferenceId, CreatedUserId, CreatedDateTime
    )
    SELECT
        gh.TenantId,
        gh.BranchId,
        gh.GRNDate,
        ''GRN'',
        gh.GRNNo,
        gl.ItemId,
        gh.WarehouseId,
        gl.BatchNo,
        gl.ExpiryDate,
        gl.ReceivedQty,
        0,
        gl.UnitCost,
        ''GRN'',
        CAST(gh.GoodsReceiptHeaderId AS NVARCHAR(40)),
        @UserId,
        SYSDATETIME()
    FROM inventory.GoodsReceiptHeaders gh
    JOIN inventory.GoodsReceiptLines gl ON gl.GoodsReceiptHeaderId = gh.GoodsReceiptHeaderId
    WHERE gh.GoodsReceiptHeaderId = @GoodsReceiptHeaderId
      AND gh.TenantId = @TenantId;

    UPDATE inventory.GoodsReceiptHeaders
    SET Status = ''POSTED'',
        UpdatedUserId = @UserId,
        UpdatedDateTime = SYSDATETIME()
    WHERE GoodsReceiptHeaderId = @GoodsReceiptHeaderId
      AND TenantId = @TenantId;

    EXEC inventory.spRebuildStockBalances @TenantId = @TenantId, @UserId = @UserId;
END
');

IF OBJECT_ID('inventory.spPostStockTransfer', 'P') IS NOT NULL DROP PROCEDURE inventory.spPostStockTransfer;
EXEC ('
CREATE PROCEDURE inventory.spPostStockTransfer
    @TenantId INT,
    @StockTransferHeaderId BIGINT,
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TransferNo NVARCHAR(40), @TransferDate DATETIME2(0), @FromWarehouseId INT, @ToWarehouseId INT, @FromBranchId INT, @ToBranchId INT;

    SELECT
        @TransferNo = TransferNo,
        @TransferDate = TransferDate,
        @FromWarehouseId = FromWarehouseId,
        @ToWarehouseId = ToWarehouseId,
        @FromBranchId = FromBranchId,
        @ToBranchId = ToBranchId
    FROM inventory.StockTransferHeaders
    WHERE StockTransferHeaderId = @StockTransferHeaderId
      AND TenantId = @TenantId;

    IF @TransferNo IS NULL THROW 50003, ''Transfer not found.'', 1;

    INSERT INTO inventory.StockLedger
    (
        TenantId, BranchId, LedgerDate, TransactionType, TransactionNo, ItemId, WarehouseId,
        QtyIn, QtyOut, UnitCost, ReferenceType, ReferenceId, CreatedUserId, CreatedDateTime
    )
    SELECT
        @TenantId, @FromBranchId, @TransferDate, ''TRANSFER_OUT'', @TransferNo,
        tl.ItemId, @FromWarehouseId,
        0, tl.TransferQty, ISNULL(i.StandardCost, 0),
        ''TRANSFER'', CAST(@StockTransferHeaderId AS NVARCHAR(40)),
        @UserId, SYSDATETIME()
    FROM inventory.StockTransferLines tl
    JOIN inventory.Items i ON i.ItemId = tl.ItemId
    WHERE tl.StockTransferHeaderId = @StockTransferHeaderId
      AND tl.TenantId = @TenantId;

    INSERT INTO inventory.StockLedger
    (
        TenantId, BranchId, LedgerDate, TransactionType, TransactionNo, ItemId, WarehouseId,
        QtyIn, QtyOut, UnitCost, ReferenceType, ReferenceId, CreatedUserId, CreatedDateTime
    )
    SELECT
        @TenantId, @ToBranchId, @TransferDate, ''TRANSFER_IN'', @TransferNo,
        tl.ItemId, @ToWarehouseId,
        tl.TransferQty, 0, ISNULL(i.StandardCost, 0),
        ''TRANSFER'', CAST(@StockTransferHeaderId AS NVARCHAR(40)),
        @UserId, SYSDATETIME()
    FROM inventory.StockTransferLines tl
    JOIN inventory.Items i ON i.ItemId = tl.ItemId
    WHERE tl.StockTransferHeaderId = @StockTransferHeaderId
      AND tl.TenantId = @TenantId;

    UPDATE inventory.StockTransferHeaders
    SET Status = ''POSTED'',
        UpdatedUserId = @UserId,
        UpdatedDateTime = SYSDATETIME()
    WHERE StockTransferHeaderId = @StockTransferHeaderId
      AND TenantId = @TenantId;

    EXEC inventory.spRebuildStockBalances @TenantId = @TenantId, @UserId = @UserId;
END
');

/* =========================================================
   8) INDEXES
CREATE INDEX IX_inventory_Items_Tenant_NameArabic ON inventory.Items (TenantId, ItemNameArabic);
CREATE INDEX IX_inventory_Suppliers_Tenant_NameArabic ON inventory.Suppliers (TenantId, SupplierNameArabic);
CREATE INDEX IX_inventory_StockLedger_Tenant_Branch_ItemWarehouseDate ON inventory.StockLedger (TenantId, BranchId, ItemId, WarehouseId, LedgerDate);
CREATE INDEX IX_inventory_ItemStockBalances_Tenant_Branch_ItemWarehouse ON inventory.ItemStockBalances (TenantId, BranchId, ItemId, WarehouseId);
CREATE INDEX IX_inventory_StockReservations_Tenant_Branch_ItemWarehouse ON inventory.StockReservations (TenantId, BranchId, ItemId, WarehouseId);
CREATE INDEX IX_inventory_ItemBatches_Tenant_Expiry ON inventory.ItemBatches (TenantId, ExpiryDate);
CREATE INDEX IX_inventory_GoodsReceiptHeaders_Tenant_GRNDate ON inventory.GoodsReceiptHeaders (TenantId, GRNDate);

COMMIT TRANSACTION;
