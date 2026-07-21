using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.People;

public class Person : SoftDeleteEntity
{
    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? SecondLastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }

    public Client? Client { get; set; }
    public Supplier? Supplier { get; set; }
    public Employee? Employee { get; set; }
}
