using NexusERP.Domain.Common;

namespace NexusERP.Domain.Entities.People;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; } = true;

    public Department? Parent { get; set; }
    public ICollection<Department> Children { get; set; } = new List<Department>();
    public ICollection<JobPosition> JobPositions { get; set; } = new List<JobPosition>();
}
