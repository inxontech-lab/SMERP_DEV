from pathlib import Path

PAGE_WIDTH = 1191
PAGE_HEIGHT = 842

entities = {
    "InvItemCategory": {
        "x": 40,
        "y": 640,
        "w": 190,
        "h": 120,
        "fields": ["PK Id", "TenantId", "Code", "Name"],
    },
    "InvItemSubCategory": {
        "x": 260,
        "y": 640,
        "w": 190,
        "h": 120,
        "fields": ["PK Id", "FK CategoryId", "TenantId", "Code", "Name"],
    },
    "InvUom": {
        "x": 480,
        "y": 640,
        "w": 170,
        "h": 120,
        "fields": ["PK Id", "TenantId", "Code", "Name"],
    },
    "InvItem": {
        "x": 230,
        "y": 430,
        "w": 260,
        "h": 180,
        "fields": [
            "PK Id", "FK CategoryId", "FK SubCategoryId", "FK BaseUOMId",
            "FK PurchaseUOMId", "FK SalesUOMId", "TenantId", "Code", "Name"
        ],
    },
    "InvItemUomconversion": {
        "x": 520,
        "y": 430,
        "w": 230,
        "h": 130,
        "fields": ["PK Id", "FK ItemId", "FK FromUOMId", "FK ToUOMId", "Factor"],
    },
    "InvSupplier": {
        "x": 780,
        "y": 640,
        "w": 170,
        "h": 120,
        "fields": ["PK Id", "TenantId", "Code", "Name"],
    },
    "InvWarehouse": {
        "x": 780,
        "y": 430,
        "w": 190,
        "h": 130,
        "fields": ["PK Id", "TenantId", "FK BranchId", "Code", "Name"],
    },
    "InvGRNHeader": {
        "x": 980,
        "y": 430,
        "w": 180,
        "h": 130,
        "fields": ["PK Id", "FK SupplierId", "FK WarehouseId", "GRNNo", "Status"],
    },
    "InvGRNLine": {
        "x": 980,
        "y": 260,
        "w": 180,
        "h": 130,
        "fields": ["PK Id", "FK GRNHeaderId", "FK ItemId", "FK UOMId", "Qty", "UnitCost"],
    },
    "InvAdjustmentHeader": {
        "x": 780,
        "y": 260,
        "w": 190,
        "h": 130,
        "fields": ["PK Id", "FK WarehouseId", "AdjustNo", "Status"],
    },
    "InvAdjustmentLine": {
        "x": 780,
        "y": 90,
        "w": 190,
        "h": 130,
        "fields": ["PK Id", "FK AdjustmentHeaderId", "FK ItemId", "FK UOMId", "QtyDelta"],
    },
    "InvTransferHeader": {
        "x": 520,
        "y": 240,
        "w": 230,
        "h": 140,
        "fields": ["PK Id", "FromWarehouseId", "ToWarehouseId", "TransferNo", "Status"],
    },
    "InvTransferLine": {
        "x": 520,
        "y": 60,
        "w": 230,
        "h": 130,
        "fields": ["PK Id", "FK TransferHeaderId", "FK ItemId", "FK UOMId", "Qty"],
    },
    "InvStockBalance": {
        "x": 260,
        "y": 220,
        "w": 230,
        "h": 130,
        "fields": ["PK Id", "FK WarehouseId", "FK ItemId", "BatchNo", "QtyOnHand"],
    },
    "InvStockTxn": {
        "x": 260,
        "y": 50,
        "w": 230,
        "h": 130,
        "fields": ["PK Id", "FK WarehouseId", "FK ItemId", "TxnType", "QtyIn", "QtyOut"],
    },
}

relations = [
    ("InvItemCategory", "InvItemSubCategory", "1:N"),
    ("InvItemCategory", "InvItem", "1:N"),
    ("InvItemSubCategory", "InvItem", "1:N"),
    ("InvUom", "InvItem", "1:N"),
    ("InvItem", "InvItemUomconversion", "1:N"),
    ("InvUom", "InvItemUomconversion", "1:N"),
    ("InvSupplier", "InvGRNHeader", "1:N"),
    ("InvWarehouse", "InvGRNHeader", "1:N"),
    ("InvGRNHeader", "InvGRNLine", "1:N"),
    ("InvItem", "InvGRNLine", "1:N"),
    ("InvUom", "InvGRNLine", "1:N"),
    ("InvWarehouse", "InvAdjustmentHeader", "1:N"),
    ("InvAdjustmentHeader", "InvAdjustmentLine", "1:N"),
    ("InvItem", "InvAdjustmentLine", "1:N"),
    ("InvUom", "InvAdjustmentLine", "1:N"),
    ("InvWarehouse", "InvTransferHeader", "1:N (from/to)"),
    ("InvTransferHeader", "InvTransferLine", "1:N"),
    ("InvItem", "InvTransferLine", "1:N"),
    ("InvUom", "InvTransferLine", "1:N"),
    ("InvWarehouse", "InvStockBalance", "1:N"),
    ("InvItem", "InvStockBalance", "1:N"),
    ("InvWarehouse", "InvStockTxn", "1:N"),
    ("InvItem", "InvStockTxn", "1:N"),
]

def esc(text: str) -> str:
    return text.replace('\\', '\\\\').replace('(', '\\(').replace(')', '\\)')

commands = []
commands.append("0.95 0.95 0.98 rg 0 0 {0} {1} re f".format(PAGE_WIDTH, PAGE_HEIGHT))
commands.append("0 0 0 RG 0 0 0 rg")

# draw relations behind boxes
for src, dst, label in relations:
    a = entities[src]
    b = entities[dst]
    ax = a["x"] + a["w"] / 2
    ay = a["y"] + a["h"] / 2
    bx = b["x"] + b["w"] / 2
    by = b["y"] + b["h"] / 2
    mx = (ax + bx) / 2
    my = (ay + by) / 2
    commands.append("0.35 0.35 0.35 RG 1 w {0:.1f} {1:.1f} m {2:.1f} {3:.1f} l S".format(ax, ay, bx, by))
    commands.append("BT /F1 8 Tf {0:.1f} {1:.1f} Td ({2}) Tj ET".format(mx + 2, my + 2, esc(label)))

# draw boxes and text
for name, meta in entities.items():
    x, y, w, h = meta["x"], meta["y"], meta["w"], meta["h"]
    commands.append("0.84 0.91 1 rg {0} {1} {2} {3} re f".format(x, y, w, h))
    commands.append("0.2 0.2 0.2 RG 1.2 w {0} {1} {2} {3} re S".format(x, y, w, h))
    commands.append("0.75 0.84 0.98 rg {0} {1} {2} 20 re f".format(x, y + h - 20, w))
    commands.append("0.2 0.2 0.2 RG {0} {1} {2} 20 re S".format(x, y + h - 20, w))
    commands.append("BT /F2 10 Tf {0} {1} Td ({2}) Tj ET".format(x + 6, y + h - 14, esc(name)))
    ty = y + h - 34
    for fld in meta["fields"]:
        commands.append("BT /F1 8 Tf {0} {1} Td ({2}) Tj ET".format(x + 6, ty, esc(f"• {fld}")))
        ty -= 11

commands.append("BT /F2 14 Tf 36 805 Td (SMERP Inventory Schema - ER Diagram) Tj ET")
commands.append("BT /F1 9 Tf 36 790 Td (Source: Domain/SaasDBModels inventory entities and EF Core mappings.) Tj ET")

stream = "\n".join(commands).encode("latin-1", errors="replace")

objs = []
objs.append(b"<< /Type /Catalog /Pages 2 0 R >>")
objs.append(b"<< /Type /Pages /Kids [3 0 R] /Count 1 >>")
objs.append(f"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PAGE_WIDTH} {PAGE_HEIGHT}] /Contents 4 0 R /Resources << /Font << /F1 5 0 R /F2 6 0 R >> >> >>".encode())
objs.append(f"<< /Length {len(stream)} >>\nstream\n".encode() + stream + b"\nendstream")
objs.append(b"<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>")
objs.append(b"<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>")

pdf = bytearray(b"%PDF-1.4\n")
offsets = [0]
for i, obj in enumerate(objs, start=1):
    offsets.append(len(pdf))
    pdf.extend(f"{i} 0 obj\n".encode())
    pdf.extend(obj)
    pdf.extend(b"\nendobj\n")

xref_pos = len(pdf)
pdf.extend(f"xref\n0 {len(objs)+1}\n".encode())
pdf.extend(b"0000000000 65535 f \n")
for off in offsets[1:]:
    pdf.extend(f"{off:010d} 00000 n \n".encode())

pdf.extend(f"trailer\n<< /Size {len(objs)+1} /Root 1 0 R >>\nstartxref\n{xref_pos}\n%%EOF\n".encode())

outputs = [Path("docs/inventory-er-diagram.pdf"), Path("Inventory-ER-Diagram.pdf")]
for out in outputs:
    out.write_bytes(pdf)
    print(f"Wrote {out}")
