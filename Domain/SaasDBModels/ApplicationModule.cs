using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class ApplicationModule
{
    public int Id { get; set; }

    public string ModuleNameEn { get; set; } = null!;

    public string? ModuleNameAr { get; set; }

    public bool Active { get; set; }
}
