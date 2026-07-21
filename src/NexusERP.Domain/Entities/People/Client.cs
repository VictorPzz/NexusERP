using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.People;

public class Client : SoftDeleteEntity
{
    public int PersonId { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public ClientStatus Status { get; set; } = ClientStatus.active;
    public string? Notes { get; set; }

    public Person Person { get; set; } = null!;
    public ICollection<ClientAddress> Addresses { get; set; } = new List<ClientAddress>();
}
