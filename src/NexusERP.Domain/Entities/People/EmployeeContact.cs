namespace NexusERP.Domain.Entities.People;

public class EmployeeContact
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string ContactType { get; set; } = string.Empty;
    public string ContactValue { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }

    public Employee Employee { get; set; } = null!;
}
