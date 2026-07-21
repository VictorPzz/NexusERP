using NexusERP.Domain.Common;

namespace NexusERP.Domain.Entities.Security;

public class SystemConfiguration : BaseEntity
{
    public string Module { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
    public bool IsActive { get; set; } = true;
}
