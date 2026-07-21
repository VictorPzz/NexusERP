using NexusERP.Domain.Entities.Purchases;

namespace NexusERP.Domain.Interfaces.Repositories.Purchases;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
