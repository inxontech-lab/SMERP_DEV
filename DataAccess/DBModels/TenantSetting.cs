using System;
using System.Collections.Generic;

namespace DataAccess.DBModels;

public partial class TenantSetting
{
    public int TenantId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public byte MoneyDecimals { get; set; }

    public byte QtyDecimals { get; set; }

    public decimal VatRateDefault { get; set; }

    public bool PricesVatInclusive { get; set; }

    public bool AllowNegativeStock { get; set; }

    public string? ReceiptPrefix { get; set; }

    public string? InvoicePrefix { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}
