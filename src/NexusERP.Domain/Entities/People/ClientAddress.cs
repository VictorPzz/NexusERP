using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.People;

public class ClientAddress : BaseEntity
{
    public int ClientId { get; set; }
    public AddressType AddressType { get; set; } = AddressType.billing;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "México";
    public bool IsPrimary { get; set; }

    public Client Client { get; set; } = null!;
}
