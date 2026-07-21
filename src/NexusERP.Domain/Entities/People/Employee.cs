using NexusERP.Domain.Common;
using NexusERP.Domain.Entities.Security;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.People;

public class Employee : SoftDeleteEntity
{
    public int PersonId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public int JobPositionId { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public EmploymentType EmploymentType { get; set; } = EmploymentType.full_time;
    public EmployeeStatus Status { get; set; } = EmployeeStatus.active;
    public decimal? Salary { get; set; }
    public int? UserId { get; set; }

    public Person Person { get; set; } = null!;
    public JobPosition JobPosition { get; set; } = null!;
    public User? User { get; set; }
    public ICollection<EmployeeContact> Contacts { get; set; } = new List<EmployeeContact>();
}
