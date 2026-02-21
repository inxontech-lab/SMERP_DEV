using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class Uom
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public byte UomType { get; set; }
}
