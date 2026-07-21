using NexusERP.Domain.Entities.Billing;

namespace NexusERP.Domain.Interfaces.Repositories.Billing;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
